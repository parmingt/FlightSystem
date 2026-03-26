using AmadeusSDK;
using FlightSystem.Services.Models;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;

    public FlightSearchService(AmadeusClient routesClient)
    {
        this.routesClient = routesClient;
    }

    public async Task<List<Models.FlightOffer>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o => {
            var flight = o.ToFlight();
            return flight;
        }).ToList();
        return flights;
    }

    public async Task<bool> ConfirmFlight(FlightOffer flight)
    {
        var amadeusOffer = flight.ToOffer();
        var confirmation = await routesClient.ConfirmFlightOffer(new List<AmadeusSDK.Models.OffersSearch.Offers> { amadeusOffer });
        return confirmation.Success ? confirmation.Data.First().Price.Total == amadeusOffer.Price.Total : false;
    }

    public async Task<Result<Models.Booking>> BookFlight(FlightOffer flight)
    {
        var amadeusOffer = flight.ToOffer();
        var confirmation = await routesClient.BookFlight(new AmadeusSDK.Models.FlightOrder() { FlightOffers = new List<AmadeusSDK.Models.OffersSearch.Offers> { amadeusOffer } });
        
        if (!confirmation.Success)
        {
            return Result<Models.Booking>.FailureResult(confirmation.FailureReason);
        }
        return Result<Models.Booking>.SuccessResult(new Models.Booking(flight, DateTime.Now, confirmation.Data.Id));
    }
}
