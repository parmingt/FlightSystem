using FlightSystem.BookingListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AmadeusSDK;
using Serilog;
using Serilog.Settings.Configuration;
using Serilog.Events;

IConfiguration configuration = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json")
     .AddUserSecrets<Program>()
     .Build();

CancellationTokenSource cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    Console.WriteLine("Cancel event triggered");
    cts.Cancel();
    eventArgs.Cancel = true;
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .ReadFrom.Configuration(configuration)
    .WriteTo.File("C:\\log\\Flights\\apptest.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton<BookingListener>()
    .AddBookingConsumer(configuration)
    .AddAmadeusClient(configuration["Amadeus:ClientId"], configuration["Amadeus:ClientSecret"])
    .BuildServiceProvider();

await serviceProvider.GetRequiredService<BookingListener>().Run(cts.Token);