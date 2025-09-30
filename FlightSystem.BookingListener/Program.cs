using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FlightSystem.BookingListener;
using FlightSystem.Kafka.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

IConfiguration configuration = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json").Build();

CancellationTokenSource cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine("Cancel event triggered");
    cts.Cancel();
    eventArgs.Cancel = true;
};

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton<BookingListener>()
    .AddBookingConsumer(configuration)
    .BuildServiceProvider();

serviceProvider.GetRequiredService<BookingListener>().Run(cts.Token);