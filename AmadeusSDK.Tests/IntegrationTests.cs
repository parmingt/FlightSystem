﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using static AmadeusSDK.Models.OffersSearch;

namespace AmadeusSDK.Tests;

[TestClass]
public sealed class IntegrationTests
{

    [TestMethod]
    public async Task SearchFlights()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }

    [TestMethod]
    public async Task FlightOffers()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        var offers = new List<Offers> { flights.First() };
        var confirmation = await client.ConfirmFlightOffer(offers);

        Assert.IsTrue(confirmation.Any());
        Assert.IsTrue(flights.First().price.total == confirmation.First().price.total);
    }
}
