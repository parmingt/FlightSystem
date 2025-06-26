using AmadeusSDK;
using AmadeusSDK.Models;
using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.Extensions.Caching.Memory;
using static AmadeusSDK.AmadeusClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FlightSystem.Services;

public class FlightSearchService
{
    private readonly AmadeusClient routesClient;
    private readonly IMemoryCache memoryCache;

    public FlightSearchService(AmadeusClient routesClient, IMemoryCache memoryCache)
    {
        this.routesClient = routesClient;
        this.memoryCache = memoryCache;
    }

    public async Task<List<Models.Flight>> SearchFlightsAsync(IataCode origin, IataCode destination, DateOnly travelDate)
    {
        var offers = await routesClient.SearchFlightsAsync(origin.ToString(), destination.ToString(), travelDate.ToDateTime(TimeOnly.MinValue));
        var flights = offers.Select(o => {
            var flight = o.ToFlight();
            memoryCache.Set(("offers", flight), o);
            return flight;
        }).ToList();
        return flights;
    }

    public async Task<bool> ConfirmFlight(Flight flight)
    {
        if (!memoryCache.TryGetValue(("offers", flight), out OffersSearch.Offers? cachedOffer)
                || cachedOffer is null)
            return false;

        var confirmation = await routesClient.ConfirmFlightOffer(new List<OffersSearch.Offers> { cachedOffer });
        return confirmation.First().price.total == cachedOffer.price.total;
    }

    public async Task<BookedFlight> BookFlight(Flight flight)
    {
        if (!memoryCache.TryGetValue(("offers", flight), out OffersSearch.Offers? cachedOffer)
                || cachedOffer is null)
            throw new Exception("Flight offer not found in cache.");

        var order = new FlightOrder()
        {
            flightOffers = new List<OffersSearch.Offers> { cachedOffer },
            travelers = new List<Traveler>
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
            },
            type = "flight-order"
        };
        var bookedOffers = await routesClient.BookFlight(order);
        var firstOffer = bookedOffers.First();
        var bookedFlight = firstOffer.ToFlight();
        return new BookedFlight(bookedFlight, DateTime.UtcNow);
    }
}
