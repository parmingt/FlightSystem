using AmadeusSDK.Models;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmadeusSDK.Models.OffersSearch;

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
}
