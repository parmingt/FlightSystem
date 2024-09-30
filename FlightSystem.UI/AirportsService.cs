using FlightSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace FlightSystem.UI;

public class AirportsService
{
    private readonly FlightContext context;

    public AirportsService(FlightContext context)
    {
        this.context = context;
    }

    public async Task<List<Airport>> SearchAirportsAsync(string query)
    {
        var data = context.Airports.Where(a =>
            EF.Functions.Like(a.Code, $"%{query}%") || EF.Functions.Like(a.Name, $"%{query}%"));
        return await data.Select(a => new Airport(a.Code, a.Name))
        .ToListAsync();
    }

}

public class Airport
{
    public Airport(string code, string name)
    {
        this.Code = code;
        this.Name = name;
    }
    public string Code { get; set; }
    public string Name { get; set; }
}
