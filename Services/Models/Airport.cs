namespace FlightSystem.Services.Models;

public record Airport(IataCode Code, string Name);

public readonly record struct IataCode(string Value)
{
    public override string ToString()
    {
        return Value.ToString();
    }
    public static IataCode Empty() => new IataCode("");
};
