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
        Assert.IsTrue(flights.First().Price.Total == confirmation.First().Price.Total);
    }

    [TestMethod]
    public async Task BookFlight()
    {
        using var serviceProvider = TestHelpers.BuildServiceCollection().BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var client = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await client.SearchFlightsAsync(origin, destination, DateTime.Now.AddDays(1));

        var confirmedOffer = await client.ConfirmFlightOffer([flights[2]]);
        var order = new FlightOrder()
        {
            FlightOffers = new List<Offers> { confirmedOffer.First() },
            Travelers = new List<Traveler>()
            {
                new Traveler()
                {
                    Id = "1",
                    DateOfBirth = "1990-10-28",
                    Name = new Name()
                    {
                        FirstName = "Peter",
                        LastName = "Armington"
                    },
                    Gender = "MALE",
                    Contact = new Contact()
                    {
                        EmailAddress = "jorge.gonzales833@telefonica.es",
                        Phones = [
                            new Phone() {
                                DeviceType = "MOBILE",
                                CountryCallingCode = "34",
                                Number = "480080076"
                            }
                        ]
                    }
                }
            }
        };
        var confirmation = await client.BookFlight(order);

        Assert.IsNotNull(confirmation);
    }
}
