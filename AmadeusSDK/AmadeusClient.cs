using FlightSystem.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static AmadeusSDK.Models;

namespace AmadeusSDK;

public class AmadeusClient : IAmadeusClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;
    private readonly IMemoryCache memoryCache;
    private readonly string tokenCacheKey = "amadeusToken";

    public AmadeusClient(HttpClient httpClient, IConfiguration configuration, IMemoryCache memoryCache)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
        this.memoryCache = memoryCache;
    }
    public async Task<List<Offers>> SearchFlightsAsync(string origin, string destination, DateTime departure, int numAdults = 1)
    {
        var client = await GetClientWithTokenAsync();
        var formattedDate = departure.ToString("yyyy-MM-dd");
        var endpoint = $"v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={formattedDate}&adults={numAdults}";
        var response = await client.GetAsync(endpoint);
        var offers = await response.Content.ReadFromJsonAsync<FlightOffersResponse>();
        return offers!.data.ToList();
    }

    private async Task<string> GetTokenAsync()
    {
        if (memoryCache.TryGetValue(tokenCacheKey, out string cacheValue))
            return cacheValue;

        var clientId = configuration["Amadeus:ClientId"];
        var clientSecret = configuration["Amadeus:ClientSecret"];
        var endpoint = "v1/security/oauth2/token";
        var dict = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId },
            { "client_secret", clientSecret }
        };
        var content = new FormUrlEncodedContent(dict);
        var response = await httpClient.PostAsync(endpoint, content);
        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<AmadeusToken>(json);
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(token.expires_in));

        memoryCache.Set(tokenCacheKey, token.access_token, cacheEntryOptions);
        return token.access_token;
    }

    private async Task<HttpClient> GetClientWithTokenAsync()
    {
        var token = await GetTokenAsync();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return httpClient;
    }

    

}
