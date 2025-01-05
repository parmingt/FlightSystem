namespace FlightSystem.Services.Models;

public record Flight(DateTime Date, Price Price, List<Segment> Segments)
{
    public IataCode Origin => Segments.First().Origin;
    public IataCode Destination => Segments.Last().Destination;
};

