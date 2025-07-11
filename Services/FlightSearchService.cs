﻿using AmadeusSDK;
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
    private readonly IProducer<string, Kafka.Models.FlightOrder> producer;

    public FlightSearchService(AmadeusClient routesClient, IMemoryCache memoryCache,
        IProducer<string, Kafka.Models.FlightOrder> producer)
    {
        this.routesClient = routesClient;
        this.memoryCache = memoryCache;
        this.producer = producer;
    }

    public async Task<List<Models.Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o => {
            var flight = o.ToFlight();
            memoryCache.Set(("offers", flight), o);
            return flight;
        }).ToList();
        return flights;
    }

    public async Task<bool> ConfirmFlight(Flight flight)
    {
        if (!memoryCache.TryGetValue(("offers", flight), out AmadeusSDK.Models.OffersSearch.Offers? cachedOffer)
                || cachedOffer is null)
            return false;

        var confirmation = await routesClient.ConfirmFlightOffer(new List<AmadeusSDK.Models.OffersSearch.Offers> { cachedOffer });
        return confirmation.First().price.total == cachedOffer.price.total;
    }

    public async Task<BookedFlight> BookFlight(Flight flight)
    {
        if (!memoryCache.TryGetValue(("offers", flight), out AmadeusSDK.Models.OffersSearch.Offers? cachedOffer)
                || cachedOffer is null)
            throw new Exception("Flight offer not found in cache.");

        string jsonString = JsonSerializer.Serialize(cachedOffer);
        var kafkaOffer = JsonSerializer.Deserialize<Kafka.Models.OffersSearch.Offers>(jsonString);
        var order = new FlightOrder()
        {
            flightOffers = [kafkaOffer]
        };
        await producer.ProduceAsync("flight-orders", new Message<string, FlightOrder>
        {
            Key = "flight-order",
            Value = order
        });
        //var bookedOffers = await routesClient.BookFlight(order);
        //var firstOffer = bookedOffers.First();
        //var bookedFlight = firstOffer.ToFlight();
        return new BookedFlight(flight, DateTime.UtcNow);
    }
}
