using AmadeusSDK.Models;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using static AmadeusSDK.Models.OffersSearch;

namespace AmadeusSDK;

public class AmadeusClient : IAmadeusClient
{
    private readonly HttpClient httpClient;
    private readonly Serilog.ILogger logger;
    private readonly string clientId;
    private readonly string clientSecret;
    private readonly string tokenCacheKey = "amadeusToken";
    private readonly IMemoryCache memoryCache;

    public AmadeusClient(IMemoryCache memoryCache, HttpClient httpClient, AmadeusClientOptions options)
    {
        this.httpClient = httpClient;
        logger = Log.ForContext<AmadeusClient>();
        clientId = options.ClientId;
        clientSecret = options.ClientSecret;
        this.memoryCache = memoryCache;
    }
    public async Task<List<Offers>> SearchFlightsAsync(string origin, string destination, DateTime departure, int numAdults = 1)
    {
        var client = await GetClientWithTokenAsync();
        var formattedDate = departure.ToString("yyyy-MM-dd");
        var endpoint = $"v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={formattedDate}&adults={numAdults}&currencyCode=USD";
        var response = await client.GetAsync(endpoint);

        await IsSuccessful<bool>(response);

        try
        {
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine(raw);
            var offers = await response.Content.ReadFromJsonAsync<FlightOffersResponse>();
            return offers!.data.ToList();
        }
        catch (Exception)
        {
            Debug.WriteLine($"Error deserializing response: {response.Content.ToString()}");
            throw;
        }
    }

    public async Task<List<Offers>> ConfirmFlightOffer(List<Offers> offersToConfirm)
    {
        var client = await GetClientWithTokenAsync();
        var endpoint = $"v1/shopping/flight-offers/pricing";

        var request = new PricingConfirmation()
        {
            data = new Data
            {
                type = "flight-offers-pricing",
                flightOffers = offersToConfirm
            }
        };

        var response = await client.PostAsJsonAsync(endpoint, request);
        await IsSuccessful<bool>(response);

        var json = await response.Content.ReadAsStringAsync();
        var confirmedOffers = JsonSerializer.Deserialize<PricingConfirmation>(json);
        return confirmedOffers.data.flightOffers;
    }

    public async Task<List<Offers>> BookFlight(FlightOrder order)
    {
        var client = await GetClientWithTokenAsync();
        var endpoint = $"v1/booking/flight-orders";

        var request = new DataWrapper<FlightOrder>(order);
        var requestJson = JsonSerializer.Serialize(request);
        var response = await client.PostAsJsonAsync(endpoint, request);

        if (!await IsSuccessful(response, request))
            return [];

        var json = await response.Content.ReadAsStringAsync();
        var confirmedOffers = JsonSerializer.Deserialize<DataWrapper<FlightOrder>>(json);
        return confirmedOffers.data.flightOffers;
    }

    public async Task<string> GetTokenAsync()
    {
        if (memoryCache.TryGetValue(tokenCacheKey, out string cacheValue))
            return cacheValue;

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

    private async Task<bool> IsSuccessful<TRequest>(HttpResponseMessage response, DataWrapper<TRequest>? request = null)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var errors = JsonSerializer.Deserialize<ErrorResponse>(errorContent);
            logger.ForContext("ResponseContent", errorContent)
                .ForContext("RequestData", JsonSerializer.Serialize(request))
                .Error("Error confirming flight offers: {StatusCode} - {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
        }
        return response.IsSuccessStatusCode;
    }

}
