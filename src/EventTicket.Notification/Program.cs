using EventTicket.Notification.Consumers;
using MassTransit;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) =>
        configuration
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341"))
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ConcertCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host("127.0.0.1", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();
