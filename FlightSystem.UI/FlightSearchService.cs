using FlightSystem.Data;
using FlightSystem.Services;
using FlightSystem.Services.Models;

namespace FlightSystem.UI;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;

    public FlightSearchService(FlightContext context, AmadeusClient routesClient)
    {
        this.routesClient = routesClient;
    }

    public async Task<List<Services.Models.Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var routes = await routesClient.SearchFlightsAsync(origin, destination, travelDate.ToDateTime(TimeOnly.MinValue));
        return routes;
        //var flights = await context.Flights.Where(f =>
        //        f.Origin.Code == origin.Code
        //        && f.Destination.Code == destination.Code
        //        && f.Departure.Date == travelDate.ToDateTime(TimeOnly.MinValue))
        //    .Select(f => new Flight(f.Departure, origin, destination))
        //    .ToListAsync();
        //return flights;
    }
}
