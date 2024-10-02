using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services;

public class AviationEdgeClient : IRoutesClient
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public AviationEdgeClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
        apiKey = configuration["AviationEdgeApiKey"];
    }
    public async Task<List<Flight>> GetRoutesAsync(Airport origin, Airport destination)
    {
        var endpoint = "routes";
        var response = await httpClient
            .GetAsync($"{endpoint}?key={apiKey}&departureIata={origin.Code}&arrivalIata={destination.Code}");
        var json = await response.Content.ReadAsStringAsync();
        var responseRoutes = JsonSerializer.Deserialize<List<RoutesResponse>>(json);
        return responseRoutes
            .Select(r => new Flight(DateTime.Parse(r.departureTime), origin, destination))
            .ToList();
    }

    private class RoutesResponse
    {
        public string airlineIata { get; set; }
        public string airlineIcao { get; set; }
        public string arrivalIata { get; set; }
        public string arrivalIcao { get; set; }
        public object arrivalTerminal { get; set; }
        public string arrivalTime { get; set; }
        public object codeshares { get; set; }
        public string departureIata { get; set; }
        public string departureIcao { get; set; }
        public object departureTerminal { get; set; }
        public string departureTime { get; set; }
        public string flightNumber { get; set; }
        public string[] regNumber { get; set; }
    }
}
