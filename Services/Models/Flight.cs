namespace FlightSystem.Services.Models;

public record Flight(DateTime Date, IataCode Origin, IataCode Destination, decimal Price);
