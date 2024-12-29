using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmadeusClient(this IServiceCollection serviceCollection
        , string clientId
        , string clientSecret)
    {
        serviceCollection.AddMemoryCache();

        serviceCollection.AddSingleton<AmadeusClientOptions>(collection =>
        {
            return new AmadeusClientOptions(clientId, clientSecret);
        });

        serviceCollection.AddHttpClient<AmadeusClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("https://test.api.amadeus.com/");
        });

        serviceCollection.AddSingleton<AmadeusClient>();
        return serviceCollection;
    }
}

public class AmadeusClientOptions
{
    public AmadeusClientOptions(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

        
    string ClientId { get; set; }
    string ClientSecret {  get; set; }
}
