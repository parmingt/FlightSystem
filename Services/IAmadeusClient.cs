using FlightSystem.Services.Models;

namespace FlightSystem.Services;

public interface IAmadeusClient
{
    Task<List<Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateTime departure, int numAdults = 1);
}