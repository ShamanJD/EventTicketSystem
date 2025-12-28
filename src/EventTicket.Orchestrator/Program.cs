using EventTicket.Contracts;
using EventTicket.Orchestrator.Hubs;
using EventTicket.Orchestrator.Sagas;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5002");

builder.Host.UseSerilog((context, configuration) =>
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

// Add services to the container.
builder.Services.AddSignalR();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Default Vite port
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

EndpointConvention.Map<RequestPayment>(new Uri("queue:payment-request"));
EndpointConvention.Map<ConfirmBooking>(new Uri("queue:booking-action"));
EndpointConvention.Map<CancelBooking>(new Uri("queue:booking-action"));

builder.Services.AddMassTransit(x =>
{
    x.AddSagaStateMachine<ConcertSaga, ConcertSagaState>()
        .InMemoryRepository();

    x.AddSagaStateMachine<BookingSaga, BookingSagaState>()
        .InMemoryRepository();

    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var host = configuration["RabbitMq:Host"] ?? "localhost";

        cfg.Host(host, "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseCors();

app.MapHub<BookingHub>("/hubs/booking");

app.Run();
