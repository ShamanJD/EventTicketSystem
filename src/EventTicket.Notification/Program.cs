using EventTicket.Notification.Consumers;
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
        services.AddMassTransit(x =>
        {
            x.AddConsumer<SendNotificationConsumer>();
            x.SetKebabCaseEndpointNameFormatter();

            x.UsingRabbitMq((context, cfg) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();

                var host = configuration["RabbitMq:Host"] ?? "localhost";

                cfg.Host(host, "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1),
                        TimeSpan.FromMilliseconds(100));
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();
