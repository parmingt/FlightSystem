using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class OffersSearch
{
    public class FlightOffersResponse
    {
        public Meta MetaData { get; set; }
        public Offers[] Data { get; set; }
    }

    public class Meta
    {
        public int Count { get; set; }
        public Links Links { get; set; }
    }

    public class Links
    {
        public string Self { get; set; }
    }

    public class Offers
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Source { get; set; }
        public bool InstantTicketingRequired { get; set; }
        public bool NonHomogeneous { get; set; }
        public bool OneWay { get; set; }
        public bool IsUpsellOffer { get; set; }
        public string LastTicketingDate { get; set; }
        public string LastTicketingDateTime { get; set; }
        public int NumberOfBookableSeats { get; set; }
        public Itinerary[] Itineraries { get; set; }
        public Price Price { get; set; }
        public Pricingoptions PricingOptions { get; set; }
        public string[] ValidatingAirlineCodes { get; set; }
        public Travelerpricing[] TravelerPricings { get; set; }
    }

    public class Price
    {
        public string Currency { get; set; }
        public string Total { get; set; }

        [JsonPropertyName("base")]
        public string? Base { get; set; }
        public Fee[] Fees { get; set; }
        public string GrandTotal { get; set; }
    }

    public class Fee
    {
        public string Amount { get; set; }
        public string Type { get; set; }
    }

    public class Pricingoptions
    {
        public string[] FareType { get; set; }
        public bool IncludedCheckedBagsOnly { get; set; }
    }

    public class Itinerary
    {
        public string Duration { get; set; }
        public Segment[] Segments { get; set; }
    }

    public class Segment
    {
        public Departure Departure { get; set; }
        public Arrival Arrival { get; set; }
        public string CarrierCode { get; set; }
        public string Number { get; set; }
        public Aircraft1 Aircraft { get; set; }
        public Operating Operating { get; set; }
        public string Duration { get; set; }
        public string Id { get; set; }
        public int NumberOfStops { get; set; }
        public bool BlacklistedInEU { get; set; }
        public Stop[]? Stops { get; set; }
    }

    public class Departure
    {
        public string IataCode { get; set; }
        public string? Terminal { get; set; }
        public DateTime At { get; set; }
    }

    public class Arrival
    {
        public string IataCode { get; set; }
        public string? Terminal { get; set; }
        public DateTime At { get; set; }
    }

    public class Aircraft1
    {
        public string Code { get; set; }
    }

    public class Operating
    {
        public string CarrierCode { get; set; }
    }

    public class Stop
    {
        public string IataCode { get; set; }
        public string Duration { get; set; }
        public DateTime ArrivalAt { get; set; }
        public DateTime DepartureAt { get; set; }
    }

    public class Travelerpricing
    {
        public string TravelerId { get; set; }
        public string FareOption { get; set; }
        public string TravelerType { get; set; }
        public Price1 Price { get; set; }
        public Faredetailsbysegment[] FareDetailsBySegment { get; set; }
    }

    public class Price1
    {
        public string Currency { get; set; }
        public string Total { get; set; }
        public string Base { get; set; }
    }

    public class Faredetailsbysegment
    {
        public string SegmentId { get; set; }
        public string Cabin { get; set; }
        public string FareBasis { get; set; }
        public string Class { get; set; }
        public Includedcheckedbags IncludedCheckedBags { get; set; }
        public string BrandedFare { get; set; }
        public string BrandedFareLabel { get; set; }
        public Amenity[] Amenities { get; set; }
        public string SliceDiceIndicator { get; set; }
    }

    public class Includedcheckedbags
    {
        public int Quantity { get; set; }
    }

    public class Amenity
    {
        public string Description { get; set; }
        public bool IsChargeable { get; set; }
        public string AmenityType { get; set; }
        public Amenityprovider AmenityProvider { get; set; }
    }

    public class Amenityprovider
    {
        public string Name { get; set; }
    }
    public class AmadeusToken
    {
        public string type { get; set; }
        public string username { get; set; }
        public string application_name { get; set; }
        public string client_id { get; set; }
        public string token_type { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string State { get; set; }
        public string Scope { get; set; }
    }
}
