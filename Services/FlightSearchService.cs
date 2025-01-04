using AmadeusSDK;
using FlightSystem.Data;
using FlightSystem.Services.Models;
using static AmadeusSDK.AmadeusClient;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;

    public FlightSearchService(AmadeusClient routesClient)
    {
        this.routesClient = routesClient;
    }

    public async Task<List<Models.Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o => o.ToFlight()).ToList();
        return flights;
    }
}
