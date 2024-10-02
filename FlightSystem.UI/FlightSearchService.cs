using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
using Services;

namespace FlightSystem.UI;

public class FlightSearchService
{
    private readonly FlightContext context;
    private readonly AviationEdgeClient aviationEdgeClient;

    public FlightSearchService(FlightContext context, AviationEdgeClient aviationEdgeClient)
    {
        this.context = context;
        this.aviationEdgeClient = aviationEdgeClient;
    }

    public async Task<List<Services.Models.Flight>> SearchFlightsAsync(Services.Models.Airport origin, Services.Models.Airport destination, DateOnly travelDate)
    {
        var routes = await aviationEdgeClient.GetRoutesAsync(origin, destination);
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
