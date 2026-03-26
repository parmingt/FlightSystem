using AmadeusSDK;
using FlightSystem.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FlightsSystem.Services.Tests;

public static class TestHelpers
{
    public static ServiceCollection BuildServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddScoped<AirportsService>();
        services.AddScoped<FlightSearchService>();
        services.AddTransient<BookingService>();
        var builder = new ConfigurationBuilder();
        builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        IConfiguration configuration = builder.Build();
        services.AddTransient((serviceProvider) =>
            FakeAmadeusClient.CreateFakeClient(serviceProvider.GetRequiredService<IMemoryCache>()));

        services.AddScoped<IConfiguration>(_ => configuration);
        return services;
    }
}
