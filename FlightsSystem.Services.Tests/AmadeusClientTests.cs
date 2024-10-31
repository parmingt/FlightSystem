using FlightSystem.Services;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace FlightsSystem.Services.Tests;

[TestClass]
public sealed class AmadeusClientTests
{
    internal static ServiceProvider? serviceProvider;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        IServiceCollection serviceCollection = new ServiceCollection();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddHttpClient<AmadeusClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("https://test.api.amadeus.com/");
        });
        var builder = new ConfigurationBuilder();
        // .AddJsonFile("appsettings.json");
        builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        IConfiguration configuration = builder.Build();

        serviceCollection.AddScoped<IConfiguration>(_ => configuration);
        serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        serviceProvider?.Dispose();
    }

    [TestMethod]
    public async Task SearchFlights()
    {
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = new IataCode("EWR");
        var destination = new IataCode("SLC");

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
