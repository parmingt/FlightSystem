using AmadeusSDK;
using AmadeusSDK.Models;
using Confluent.Kafka;
using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using FlightOrder = FlightSystem.Services.Models.FlightOrder;
using System.Text.Json;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;
    private readonly IMemoryCache memoryCache;

    public FlightSearchService(AmadeusClient routesClient, IMemoryCache memoryCache)
    {
        this.routesClient = routesClient;
        this.memoryCache = memoryCache;
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
        var amadeusOffer = flight.ToOffer();
        var confirmation = await routesClient.ConfirmFlightOffer(new List<AmadeusSDK.Models.OffersSearch.Offers> { amadeusOffer });
        return confirmation.First().Price.Total == amadeusOffer.Price.Total;
    }

    public async Task<BookedFlight> BookFlight(FlightOffer flight)
    {
        var amadeusOffer = flight.ToOffer();
        var confirmation = await routesClient.BookFlight(new AmadeusSDK.Models.FlightOrder() { FlightOffers = new List<AmadeusSDK.Models.OffersSearch.Offers> { amadeusOffer } });
        return new BookedFlight(flight, DateTime.Now, confirmation.Id);
    }
}
