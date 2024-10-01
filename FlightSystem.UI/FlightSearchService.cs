using FlightSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace FlightSystem.UI;

public class FlightSearchService
{
    private readonly FlightContext context;

    public FlightSearchService(FlightContext context)
    {
        this.context = context;
    }

    public async Task<List<Flight>> SearchFlightsAsync(Airport origin, Airport destination, DateOnly travelDate)
    {
        var flights = await context.Flights.Where(f =>
                f.Origin.Code == origin.Code
                && f.Destination.Code == destination.Code
                && f.Departure.Date == travelDate.ToDateTime(TimeOnly.MinValue))
            .Select(f => new Flight(f.Departure, origin, destination))
            .ToListAsync();
        return flights;
    }
}
