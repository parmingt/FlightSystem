using AmadeusSDK.Models;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmadeusSDK.Models.OffersSearch;

namespace FlightSystem.Services;

public static class AmadeusModelExtensions
{
    public static Models.FlightOffer ToFlight(this Offers offer)
    {
        return new FlightOffer(offer.Itineraries[0].Segments[0].Departure.At
            , new Models.Price(decimal.Parse(offer.Price.Total), offer.Price.Currency, decimal.Parse(offer.Price.Base))
            , offer.Itineraries[0].Segments.ToList().Select(s =>
                new Models.Segment(s.CarrierCode
                , s.Number
                , new IataCode(s.Departure.IataCode)
                , new IataCode(s.Arrival.IataCode)
                , s.Departure.At
                , s.Id)).ToList(), 
            offer.Id, 
            offer.TravelerPricings.Select(t => t.ToTravelerPricing()).ToList(),
            offer.Source,
            offer.ValidatingAirlineCodes.ToList()
        );
    }

    public static Offers ToOffer(this FlightOffer flight)
    {
        return new Offers()
        {
            Type = "flight-offer",
            Id = flight.OfferId,
            Price = new OffersSearch.Price
            {
                Total = flight.Price.Total.ToString(),
                Currency = flight.Price.Currency,
                Base = flight.Price.Base.ToString()
            },
            Itineraries = [
                new Itinerary
                {
                    Segments = flight.Segments.Select(s => new OffersSearch.Segment
                    {
                        CarrierCode = s.CarrierCode,
                        Number = s.Number,
                        Departure = new Departure
                        {
                            IataCode = s.Origin.ToString(),
                            At = s.Departure
                        },
                        Arrival = new Arrival
                        {
                            IataCode = s.Destination.Value.ToString(),
                            At = s.Departure
                        },
                        Id = s.Id
                    }).ToArray()
                }
            ],
            TravelerPricings = flight.TravelerPricings.Select(t => t.ToAmadeusTravelerPricing()).ToArray(),
            Source = flight.Source,
            ValidatingAirlineCodes = flight.ValidatingAirlineCodes.ToArray()
        };
    }

    private static TravelerPricing ToTravelerPricing(this Travelerpricing travelerpricing)
    {
        return new TravelerPricing(travelerpricing.TravelerId, travelerpricing.FareOption, travelerpricing.TravelerType, 
            new Models.Price(decimal.Parse(travelerpricing.Price.Total), travelerpricing.Price.Currency, decimal.Parse(travelerpricing.Price.Base)),
            travelerpricing.FareDetailsBySegment.Select(d => d.ToFareDetailBySegment()).ToList());
    }

    private static Travelerpricing ToAmadeusTravelerPricing(this TravelerPricing travelerPricing)
    {
        return new Travelerpricing
        {
            TravelerId = travelerPricing.TravelerId,
            FareOption = travelerPricing.FareOption,
            TravelerType = travelerPricing.TravelerType,
            Price = new Price1
            {
                Total = travelerPricing.Price.Total.ToString(),
                Currency = travelerPricing.Price.Currency,
                Base = travelerPricing.Price.Base.ToString()
            },
            FareDetailsBySegment = travelerPricing.FareDetails.Select(d => d.ToAmadeusFareDetailBySegment()).ToArray()
        };
    }

    private static FareDetailBySegment ToFareDetailBySegment(this Faredetailsbysegment amadeusFareDetails)
    {
        return new FareDetailBySegment(
            amadeusFareDetails.SegmentId,
            amadeusFareDetails.Cabin,
            amadeusFareDetails.FareBasis,
            amadeusFareDetails.BrandedFare,
            amadeusFareDetails.Class,
            amadeusFareDetails.IncludedCheckedBags.Quantity
        );
    }

    private static Faredetailsbysegment ToAmadeusFareDetailBySegment(this FareDetailBySegment fareDetail)
    {
        return new Faredetailsbysegment()
        {
            SegmentId = fareDetail.SegmentId,
            Cabin = fareDetail.Cabin,
            FareBasis = fareDetail.FareBasis,
            BrandedFare = fareDetail.BrandedFare,
            Class = fareDetail.Class,
            IncludedCheckedBags = new Includedcheckedbags
            {
                Quantity = fareDetail.IncludedCheckedBags
            }
        };
    }
}
