using System.Security.Cryptography.X509Certificates;

namespace FlightSystem.Services.Models;

public record FlightOffer(
    DateTime Date, 
    Price Price, 
    List<Segment> Segments, 
    string offerId,
    List<TravelerPricing> TravelerPricings)
{
    public IataCode Origin => Segments.First().Origin;
    public IataCode Destination => Segments.Last().Destination;
};

