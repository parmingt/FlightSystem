using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class FlightOrder
{
    public string Type { get; set; } = "flight-order";
    public List<OffersSearch.Offers> FlightOffers { get; set; } = [];
    public List<Traveler>? Travelers { get; set; }
    public string? Id { get; set; }
    public string? QueuingOfficeId { get; set; }
}
