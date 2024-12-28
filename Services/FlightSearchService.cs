using AmadeusSDK;
using FlightSystem.Data;
using FlightSystem.Services.Models;
using static AmadeusSDK.AmadeusClient;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;

    public FlightSearchService(FlightContext context, AmadeusClient routesClient)
    {
        this.routesClient = routesClient;
    }

    public async Task<List<Models.Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o =>
            new Flight(o.itineraries[0].segments[0].departure.at, origin, destination
                , new FlightPrice(Decimal.Parse(o.price.total), o.price.currency)
                , o.itineraries[0].segments.ToList().Select(s =>
                    new Models.Segment(s.carrierCode
                    , s.number
                    , new IataCode(s.departure.iataCode)
                    , new IataCode(s.arrival.iataCode)
                    , s.departure.at)).ToList()
            )).ToList();
        return flights;
    }
}
