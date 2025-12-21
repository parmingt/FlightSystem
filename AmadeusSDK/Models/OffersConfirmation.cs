using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AmadeusSDK.Models.OffersSearch;

namespace AmadeusSDK.Models;

public class PricingConfirmation
{
    public Data Data { get; set; }
}

public class Data
{
    public string Type { get; set; }
    public List<Offers> FlightOffers { get; set; }
}
