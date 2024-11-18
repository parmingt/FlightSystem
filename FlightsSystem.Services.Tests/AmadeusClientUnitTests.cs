using FlightSystem.Services.Models;
using FlightSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightsSystem.Services.Tests;

[TestClass]
public class AmadeusClientUnitTests
{

    [TestMethod]
    public async Task SearchFlights()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = new IataCode("EWR");
        var destination = new IataCode("SLC");

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
