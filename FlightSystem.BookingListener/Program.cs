using FlightSystem.BookingListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AmadeusSDK;

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
    .AddAmadeusClient(configuration["Amadeus:ClientId"], configuration["Amadeus:ClientSecret"])
    .BuildServiceProvider();

serviceProvider.GetRequiredService<BookingListener>().Run(cts.Token);