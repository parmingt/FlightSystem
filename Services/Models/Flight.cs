namespace FlightSystem.Services.Models;

public record Flight(DateTime Date, IataCode Origin, IataCode Destination, FlightPrice Price, List<Segment> Segments);

public record FlightPrice(decimal Total, string Currency)
{
    public override string ToString()
    {
        return $"{Total} {Currency}";
    }
};
