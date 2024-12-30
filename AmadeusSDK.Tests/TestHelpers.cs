using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Tests;

public static class TestHelpers
{
    public static ServiceCollection BuildServiceCollection()
    {
        var builder = new ConfigurationBuilder();
        // .AddJsonFile("appsettings.json");
        builder.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
        var configuration = builder.Build();

        var services = new ServiceCollection();
        services.AddAmadeusClient(configuration["Amadeus:ClientId"], configuration["Amadeus:ClientSecret"]);

        return services;
    }
}
