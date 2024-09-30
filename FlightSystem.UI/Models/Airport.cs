namespace FlightSystem.UI;

public class Airport
{
    public Airport(string code, string name)
    {
        Code = code;
        Name = name;
    }
    public string Code { get; set; }
    public string Name { get; set; }
}
