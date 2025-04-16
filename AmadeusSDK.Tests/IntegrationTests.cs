using AmadeusSDK.Models;
using Microsoft.Extensions.DependencyInjection;
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

    [TestMethod]
    public async Task BookFlight()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now);

        var order = new FlightOrder()
        {
            flightOffers = new List<Offers> { flights.First() },
            travelers = new List<Traveler>()
            {
                new Traveler()
                {
                    id = "1",
                    dateOfBirth = "1990-10-28",
                    name = new Name()
                    {
                        firstName = "Peter",
                        lastName = "Armington"
                    },
                    gender = "MALE",
                    contact = new Contact()
                    {
                        emailAddress = "jorge.gonzales833@telefonica.es",
                        phones = [
                            new Phone() {
                                deviceType = "MOBILE",
                                countryCallingCode = "34",
                                number = "480080076"
                            }
                        ]
                    }
                }
            }
        };
        var confirmation = await client.BookFlight(order);

        Assert.IsTrue(confirmation.Any());
        Assert.IsTrue(flights.First().price.total == confirmation.First().price.total);
    }
}
