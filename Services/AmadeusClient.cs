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

namespace FlightSystem.Services;

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
    public async Task<List<Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateTime departure, int numAdults = 1)
    {
        var client = await GetClientWithTokenAsync();
        var formattedDate = departure.ToString("yyyy-MM-dd");
        var endpoint = $"v2/shopping/flight-offers?originLocationCode={origin}&destinationLocationCode={destination}&departureDate={formattedDate}&adults={numAdults}";
        var response = await client.GetAsync(endpoint);
        var offers = await response.Content.ReadFromJsonAsync<FlightOffersResponse>();
        var flights = offers.data.Select(o =>
            new Flight(o.itineraries[0].segments[0].departure.at, origin, destination, Decimal.Parse(o.price.total)))
            .ToList();
        return flights;
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

    public class FlightOffersResponse
    {
        public Meta meta { get; set; }
        public Offers[] data { get; set; }
    }

    public class Meta
    {
        public int count { get; set; }
        public Links links { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }

    public class Offers
    {
        public string type { get; set; }
        public string id { get; set; }
        public string source { get; set; }
        public bool instantTicketingRequired { get; set; }
        public bool nonHomogeneous { get; set; }
        public bool oneWay { get; set; }
        public bool isUpsellOffer { get; set; }
        public string lastTicketingDate { get; set; }
        public string lastTicketingDateTime { get; set; }
        public int numberOfBookableSeats { get; set; }
        public Itinerary[] itineraries { get; set; }
        public Price price { get; set; }
        public Pricingoptions pricingOptions { get; set; }
        public string[] validatingAirlineCodes { get; set; }
        public Travelerpricing[] travelerPricings { get; set; }
    }

    public class Price
    {
        public string currency { get; set; }
        public string total { get; set; }
        public string _base { get; set; }
        public Fee[] fees { get; set; }
        public string grandTotal { get; set; }
    }

    public class Fee
    {
        public string amount { get; set; }
        public string type { get; set; }
    }

    public class Pricingoptions
    {
        public string[] fareType { get; set; }
        public bool includedCheckedBagsOnly { get; set; }
    }

    public class Itinerary
    {
        public string duration { get; set; }
        public Segment[] segments { get; set; }
    }

    public class Segment
    {
        public Departure departure { get; set; }
        public Arrival arrival { get; set; }
        public string carrierCode { get; set; }
        public string number { get; set; }
        public Aircraft1 aircraft { get; set; }
        public Operating operating { get; set; }
        public string duration { get; set; }
        public string id { get; set; }
        public int numberOfStops { get; set; }
        public bool blacklistedInEU { get; set; }
        public Stop[] stops { get; set; }
    }

    public class Departure
    {
        public string iataCode { get; set; }
        public string terminal { get; set; }
        public DateTime at { get; set; }
    }

    public class Arrival
    {
        public string iataCode { get; set; }
        public string terminal { get; set; }
        public DateTime at { get; set; }
    }

    public class Aircraft1
    {
        public string code { get; set; }
    }

    public class Operating
    {
        public string carrierCode { get; set; }
    }

    public class Stop
    {
        public string iataCode { get; set; }
        public string duration { get; set; }
        public DateTime arrivalAt { get; set; }
        public DateTime departureAt { get; set; }
    }

    public class Travelerpricing
    {
        public string travelerId { get; set; }
        public string fareOption { get; set; }
        public string travelerType { get; set; }
        public Price1 price { get; set; }
        public Faredetailsbysegment[] fareDetailsBySegment { get; set; }
    }

    public class Price1
    {
        public string currency { get; set; }
        public string total { get; set; }
        public string _base { get; set; }
    }

    public class Faredetailsbysegment
    {
        public string segmentId { get; set; }
        public string cabin { get; set; }
        public string fareBasis { get; set; }
        public string _class { get; set; }
        public Includedcheckedbags includedCheckedBags { get; set; }
        public string brandedFare { get; set; }
        public string brandedFareLabel { get; set; }
        public Amenity[] amenities { get; set; }
        public string sliceDiceIndicator { get; set; }
    }

    public class Includedcheckedbags
    {
        public int quantity { get; set; }
    }

    public class Amenity
    {
        public string description { get; set; }
        public bool isChargeable { get; set; }
        public string amenityType { get; set; }
        public Amenityprovider amenityProvider { get; set; }
    }

    public class Amenityprovider
    {
        public string name { get; set; }
    }
    public class AmadeusToken
    {
        public string type { get; set; }
        public string username { get; set; }
        public string application_name { get; set; }
        public string client_id { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string state { get; set; }
        public string scope { get; set; }
    }

}
