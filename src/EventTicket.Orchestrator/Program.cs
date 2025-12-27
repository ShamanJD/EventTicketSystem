using EventTicket.Contracts;
using EventTicket.Orchestrator.Sagas;
using MassTransit;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(context.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"))
    .ConfigureServices(services =>
    {
        EndpointConvention.Map<RequestPayment>(new Uri("queue:payment-request"));
        EndpointConvention.Map<ConfirmBooking>(new Uri("queue:booking-action"));
        EndpointConvention.Map<CancelBooking>(new Uri("queue:booking-action"));

        services.AddMassTransit(x =>
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
    })
    .Build();

await host.RunAsync();
