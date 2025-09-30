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
    internal static Models.FlightOffer ToFlight(this Offers offer)
    {
        return new FlightOffer(offer.itineraries[0].segments[0].departure.at
            , new Models.Price(Decimal.Parse(offer.price.total), offer.price.currency)
            , offer.itineraries[0].segments.ToList().Select(s =>
                new Models.Segment(s.carrierCode
                , s.number
                , new IataCode(s.departure.iataCode)
                , new IataCode(s.arrival.iataCode)
                , s.departure.at
                , s.id)).ToList(), 
            offer.id, 
            offer.travelerPricings.Select(t => t.ToTravelerPricing()).ToList(),
            offer.source,
            offer.validatingAirlineCodes.ToList()
        );
    }

    internal static Offers ToOffer(this FlightOffer flight)
    {
        return new Offers()
        {
            type = "flight-offer",
            id = flight.OfferId,
            price = new AmadeusSDK.Models.OffersSearch.Price
            {
                total = flight.Price.Total.ToString(),
                currency = flight.Price.Currency
            },
            itineraries = [
                new Itinerary
                {
                    segments = flight.Segments.Select(s => new OffersSearch.Segment
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
                            iataCode = s.Destination.Value.ToString(),
                            at = s.Departure
                        },
                        id = s.Id
                    }).ToArray()
                }
            ],
            travelerPricings = flight.TravelerPricings.Select(t => t.ToAmadeusTravelerPricing()).ToArray(),
            source = flight.Source,
            validatingAirlineCodes = flight.ValidatingAirlineCodes.ToArray()
        };
    }

    private static TravelerPricing ToTravelerPricing(this Travelerpricing travelerpricing)
    {
        return new TravelerPricing(travelerpricing.travelerId, travelerpricing.fareOption, travelerpricing.travelerType, 
            new Models.Price(Decimal.Parse(travelerpricing.price.total), travelerpricing.price.currency),
            travelerpricing.fareDetailsBySegment.Select(d => d.ToFareDetailBySegment()).ToList());
    }

    private static Travelerpricing ToAmadeusTravelerPricing(this TravelerPricing travelerPricing)
    {
        return new Travelerpricing
        {
            travelerId = travelerPricing.TravelerId,
            fareOption = travelerPricing.FareOption,
            travelerType = travelerPricing.TravelerType,
            price = new Price1
            {
                total = travelerPricing.Price.Total.ToString(),
                currency = travelerPricing.Price.Currency
            },
            fareDetailsBySegment = travelerPricing.FareDetails.Select(d => d.ToAmadeusFareDetailBySegment()).ToArray()
        };
    }

    private static FareDetailBySegment ToFareDetailBySegment(this Faredetailsbysegment amadeusFareDetails)
    {
        return new FareDetailBySegment(
            amadeusFareDetails.segmentId,
            amadeusFareDetails.cabin,
            amadeusFareDetails.fareBasis,
            amadeusFareDetails.brandedFare,
            amadeusFareDetails._class,
            amadeusFareDetails.includedCheckedBags.quantity
        );
    }

    private static Faredetailsbysegment ToAmadeusFareDetailBySegment(this FareDetailBySegment fareDetail)
    {
        return new Faredetailsbysegment()
        {
            segmentId = fareDetail.SegmentId,
            cabin = fareDetail.Cabin,
            fareBasis = fareDetail.FareBasis,
            brandedFare = fareDetail.BrandedFare,
            _class = fareDetail.Class,
            includedCheckedBags = new Includedcheckedbags
            {
                quantity = fareDetail.IncludedCheckedBags
            }
        };
    }

        //internal static Offers FromOffer(this Models.Offer offer)
        //{
        //    return new Offers
        //    {
        //        price = new OffersSearch.Price
        //        {
        //            total = offer.Price.Total.ToString(),
        //            currency = offer.Price.Currency
        //        },
        //        itineraries = [
        //            new Itinerary
        //            {
        //                segments = offer.Segments.Select(s => new OffersSearch.Segment
        //                {
        //                    carrierCode = s.CarrierCode,
        //                    number = s.Number,
        //                    departure = new Departure
        //                    {
        //                        iataCode = s.Origin.ToString(),
        //                        at = s.Departure
        //                    },
        //                    arrival = new Arrival
        //                    {
        //                        iataCode = s.Destination.ToString(),
        //                        at = s.Departure.AddHours(2) // Assuming a fixed duration for simplicity
        //                    }
        //                }).ToArray()
        //            }
        //        ]
        //    };
        //}

        //internal static Offer ToOffer(this OffersSearch.Offers offer)
        //{
        //    return new Offer(
        //        new Price(Decimal.Parse(offer.price.total), offer.price.currency),
        //        offer.itineraries[0].segments.ToList().Select(s =>
        //            new Segment(s.carrierCode
        //            , s.number
        //            , new IataCode(s.departure.iataCode)
        //            , new IataCode(s.arrival.iataCode)
        //            , s.departure.at)).ToList()
        //        );
        //}
    }
