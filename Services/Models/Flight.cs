using System.Security.Cryptography.X509Certificates;

namespace FlightSystem.Services.Models;

public record Flight(DateTime Date, Price Price, List<Segment> Segments, string offerId)
{
    public IataCode Origin => Segments.First().Origin;
    public IataCode Destination => Segments.Last().Destination;
};

