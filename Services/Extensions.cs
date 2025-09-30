using AmadeusSDK.Models;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmadeusSDK.Models.OffersSearch;
using Price = FlightSystem.Services.Models.Price;
using Segment = FlightSystem.Services.Models.Segment;

namespace FlightSystem.Services;

internal static class Extensions
{
    internal static Models.Flight ToFlight(this Offers offer)
    {
        return new Flight(offer.itineraries[0].segments[0].departure.at
            , new Models.Price(Decimal.Parse(offer.price.total), offer.price.currency)
            , offer.itineraries[0].segments.ToList().Select(s =>
                new Models.Segment(s.carrierCode
                , s.number
                , new IataCode(s.departure.iataCode)
                , new IataCode(s.arrival.iataCode)
                , s.departure.at)).ToList()
            , offer.id);
    }

    internal static Offers FromOffer(this Models.Offer offer)
    {
        return new Offers
        {
            price = new OffersSearch.Price
            {
                total = offer.Price.Total.ToString(),
                currency = offer.Price.Currency
            },
            itineraries = [
                new Itinerary
                {
                    segments = offer.Segments.Select(s => new OffersSearch.Segment
                    {
                        carrierCode = s.CarrierCode,
                        number = s.Number,
                        departure = new Departure
                        {
                            iataCode = s.Origin.ToString(),
                            at = s.Departure
                        },
                        arrival = new Arrival
                        {
                            iataCode = s.Destination.ToString(),
                            at = s.Departure.AddHours(2) // Assuming a fixed duration for simplicity
                        }
                    }).ToArray()
                }
            ]
        };
    }

    internal static Offer ToOffer(this OffersSearch.Offers offer)
    {
        return new Offer(
            new Price(Decimal.Parse(offer.price.total), offer.price.currency),
            offer.itineraries[0].segments.ToList().Select(s =>
                new Segment(s.carrierCode
                , s.number
                , new IataCode(s.departure.iataCode)
                , new IataCode(s.arrival.iataCode)
                , s.departure.at)).ToList()
            );
    }
}
