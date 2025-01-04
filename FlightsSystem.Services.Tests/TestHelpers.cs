using AmadeusSDK;
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

public static class TestHelpers
{
    public static ServiceCollection BuildServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddScoped<AirportsService>();
        var builder = new ConfigurationBuilder();
        // .AddJsonFile("appsettings.json");
        builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        IConfiguration configuration = builder.Build();
        services.AddAmadeusClient(configuration["Amadeus:ClientId"]
            , configuration["Amadeus:ClientSecret"]);

        services.AddScoped<IConfiguration>(_ => configuration);
        return services;
    }
}
