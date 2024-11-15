using FlightSystem.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlightsSystem.Services.Tests;

[TestClass]
public static class TestHelpers
{
    internal static ServiceProvider? ServiceProvider;

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
        ServiceProvider = serviceCollection.BuildServiceProvider();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        ServiceProvider?.Dispose();
    }
}
