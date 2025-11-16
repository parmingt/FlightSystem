using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;

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
        serviceCollection.AddSingleton<IAmadeusClient>(serviceProvider =>
            serviceProvider.GetRequiredService<AmadeusClient>());
        return serviceCollection;
    }
}