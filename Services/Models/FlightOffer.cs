using System.Security.Cryptography.X509Certificates;

namespace FlightSystem.Services.Models;

public record FlightOffer(
    DateTime Date, 
    Price Price, 
    List<Segment> Segments, 
    string OfferId,
    List<TravelerPricing> TravelerPricings,
    string Source,
    List<string> ValidatingAirlineCodes)
{
    public IataCode Origin => Segments.First().Origin;
    public IataCode Destination => Segments.Last().Destination;
};

