using FlightSystem.Services;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace FlightsSystem.Services.Tests;

[TestClass]
public sealed class AmadeusClientTests
{

    [TestMethod]
    public async Task SearchFlights()
    {
        using var scope = TestHelpers.ServiceProvider.CreateScope();
        var client = TestHelpers.ServiceProvider.GetRequiredService<AmadeusClient>();

        var origin = new IataCode("EWR");
        var destination = new IataCode("SLC");

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
