namespace FlightSystem.Services.Models;

public record Flight(DateTime Date, Airport Origin, Airport Destination, decimal Price);
