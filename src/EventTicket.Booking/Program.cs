using EventTicket.Booking.Consumers;
using EventTicket.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(host.Configuration.GetConnectionString("DefaultConnection")));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BookingActionConsumer>();
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

                cfg.ConfigureEndpoints(context);
            });
        });
    })
    .Build();

await host.RunAsync();
