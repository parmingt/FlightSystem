using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmadeusSDK.Models;

public class FlightOrder
{
    public string type { get; set; } = "flight-order";
    public List<OffersSearch.Offers> flightOffers { get; set; }
    public List<Traveler> travelers { get; set; }
}
