using AmadeusSDK;
using AmadeusSDK.Models;
using Confluent.Kafka;
using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using FlightSystem.Kafka.Models;
using FlightOrder = FlightSystem.Kafka.Models.FlightOrder;
using System.Text.Json;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;
    private readonly IMemoryCache memoryCache;
    private readonly IProducer<string, FlightOrder> producer;

    public FlightSearchService(AmadeusClient routesClient, IMemoryCache memoryCache,
        IProducer<string, FlightOrder> producer)
    {
        this.routesClient = routesClient;
        this.memoryCache = memoryCache;
        this.producer = producer;
    }

    public async Task<List<Models.FlightOffer>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o => {
            var flight = o.ToFlight();
            memoryCache.Set(("offers", flight), o);
            return flight;
        }).ToList();
        return flights;
    }

    public async Task<bool> ConfirmFlight(FlightOffer flight)
    {
        //if (!memoryCache.TryGetValue(("offers", flight), out AmadeusSDK.Models.OffersSearch.Offers? cachedOffer)
        //        || cachedOffer is null)
        //    return false;

        var amadeusOffer = flight.ToOffer();
        var confirmation = await routesClient.ConfirmFlightOffer(new List<AmadeusSDK.Models.OffersSearch.Offers> { amadeusOffer });
        return confirmation.First().price.total == amadeusOffer.price.total;
    }

    public async Task<BookedFlight> BookFlight(FlightOffer flight)
    {
        if (!memoryCache.TryGetValue(("offers", flight), out AmadeusSDK.Models.OffersSearch.Offers? cachedOffer)
                || cachedOffer is null)
            throw new Exception("Flight offer not found in cache.");

        string jsonString = JsonSerializer.Serialize(cachedOffer);
        var kafkaOffer = JsonSerializer.Deserialize<Kafka.Models.OffersSearch.Offers>(jsonString
            , new JsonSerializerOptions()
            {
                RespectNullableAnnotations = true
            });
        var order = new FlightOrder()
        {
            flightOffers = [kafkaOffer]
        };
        await producer.ProduceAsync("flight-orders", new Message<string, FlightOrder>
        {
            Key = "flight-order",
            Value = order
        });

        return new BookedFlight(flight, DateTime.UtcNow);
    }
}
