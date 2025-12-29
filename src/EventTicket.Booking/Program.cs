using EventTicket.Application.Services;
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
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("BookingConnection")));

        services.AddScoped<IBookingDbContext>(provider => provider.GetRequiredService<BookingDbContext>());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<BookingActionConsumer>();
            x.AddConsumer<BookingCreatedConsumer>();
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
