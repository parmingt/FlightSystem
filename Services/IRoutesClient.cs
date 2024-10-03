using FlightSystem.Services.Models;

namespace FlightSystem.Services
{
    public interface IRoutesClient
    {
        Task<List<Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateTime departure, int numAdults = 1);
    }
}