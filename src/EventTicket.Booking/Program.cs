using System.Reflection;
using EventTicket.Infrastructure;
using EventTicket.Booking.Controllers;
using EventTicket.Booking.Middleware;
using EventTicket.Booking.Middleware;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using EventTicket.Application.Services;
using EventTicket.Application.Features.Bookings.Commands.CreateBooking;
using EventTicket.Booking.Consumers; // Assuming consumers namespace
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RabbitMQ.Client;
// Using EventTicket.Booking.Consumers needed? The original Booking/Program.cs had Consumers.
// Let's verify namespace.

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

var bookingConnectionString = builder.Configuration.GetConnectionString("BookingConnection");

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Booking API", Version = "v1" });
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
builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseNpgsql(bookingConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    });
});
builder.Services.AddScoped<IBookingDbContext>(provider => provider.GetRequiredService<BookingDbContext>());

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateBookingCommand).Assembly));

// gRPC Client -> Calling Concerts Service
// Concerts Service running on port 7001 (https) eventually
var concertsGrpcAddress = builder.Configuration["GrpcServices:Concerts"] ?? "https://localhost:7001";
builder.Services.AddGrpcClient<EventTicket.Contracts.Protos.ConcertGrpc.ConcertGrpcClient>(options =>
{
    options.Address = new Uri(concertsGrpcAddress);
});

// MassTransit (Consumers)
builder.Services.AddMassTransit(x =>
{
    // If BookingService has consumers, register them here.
    // Original Booking Program.cs had BookingActionConsumer, BookingCreatedConsumer
    // We need to make sure we reference them correctly.
    // Assuming they are in EventTicket.Booking.Consumers namespace.
    // Use AppDomain scan or explicit? Explicit is safer if we know types.
    // But I don't have types imported yet in thought process. I'll scan assembly.

    var entryAssembly = Assembly.GetExecutingAssembly();
    x.AddConsumers(entryAssembly); 

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration["RabbitMq:Host"] ?? "localhost";
        cfg.Host(host, "/", h => { h.Username("guest"); h.Password("guest"); });
        cfg.ConfigureEndpoints(context);
    });
});

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

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddNpgSql(bookingConnectionString!)
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
