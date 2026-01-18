using EventTicket.Infrastructure;
using EventTicket.Service.Concerts.GrpcServices;
using EventTicket.Service.Concerts.Middleware;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using EventTicket.Application.Services;
using EventTicket.Application.Features.Concerts.Commands.CreateConcert;
using EventTicket.Application.Features.Concerts.Queries.GetConcertById;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = false;
    options.ValidateOnBuild = false;
});

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

var concertsConnectionString = builder.Configuration.GetConnectionString("ConcertsConnection");

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Concerts API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and your token."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// DB Context
builder.Services.AddDbContext<ConcertsDbContext>(options =>
{
    options.UseNpgsql(concertsConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(ConcertsDbContext).Assembly.FullName);
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
});
builder.Services.AddScoped<IConcertsDbContext>(provider => provider.GetRequiredService<ConcertsDbContext>());

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
});

// MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<ConcertsDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);
        o.UsePostgres();
        o.UseBusOutbox();
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration["RabbitMq:Host"] ?? "localhost";
        cfg.Host(host, "/", h => { h.Username("guest"); h.Password("guest"); });
        cfg.ConfigureEndpoints(context);
    });
});

// MediatR (Concerts Assembly)
// We register from CreateConcertCommand assembly which is EventTicket.Application
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateConcertCommand).Assembly));

builder.Services.AddControllers();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });
builder.Services.AddGrpc();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddNpgSql(concertsConnectionString!)
    .AddRabbitMQ(
        $"amqp://guest:guest@{(builder.Configuration["RabbitMq:Host"] ?? "localhost")}:5672",
        name: "rabbitmq"
    );

var app = builder.Build();

app.MapHealthChecks("/health/liveness", new HealthCheckOptions { Predicate = (check) => check.Name == "self" });
app.MapHealthChecks("/health/readiness");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthentication(); // JWT likely not needed for Concerts depending on requirements, but let's keep boilerplate if needed later
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<ConcertGrpcService>();

app.Run();
