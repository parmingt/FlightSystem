using FlightSystem.Data;
using FlightSystem.Services.Models;
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

    public async Task<List<Services.Models.Airport>> SearchAirportsAsync(string query)
    {
        var data = context.Airports.Where(a =>
            EF.Functions.Like(a.Code, $"%{query}%") || EF.Functions.Like(a.Name, $"%{query}%"));
        return await data.Select(a => new Services.Models.Airport(new IataCode(a.Code), a.Name)).ToListAsync();
    }

}
