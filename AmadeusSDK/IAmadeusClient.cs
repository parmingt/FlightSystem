using AmadeusSDK.Models;
using static AmadeusSDK.Models.OffersSearch;

namespace AmadeusSDK;

public interface IAmadeusClient
{
    Task<List<Offers>> SearchFlightsAsync(string origin, string destination, DateTime departure, int numAdults = 1);
    Task<FlightOrder> BookFlight(FlightOrder order);
}