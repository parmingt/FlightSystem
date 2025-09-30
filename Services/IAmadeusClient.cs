using FlightSystem.Services.Models;

namespace FlightSystem.Services;

public interface IAmadeusClient
{
    Task<List<FlightOffer>> SearchFlightsAsync(IataCode origin, IataCode destination, DateTime departure, int numAdults = 1);
}