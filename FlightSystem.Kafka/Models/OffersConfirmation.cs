using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FlightSystem.Kafka.Models.OffersSearch;

namespace FlightSystem.Kafka.Models;

public class PricingConfirmation
{
    public Data data { get; set; }
}

public class Data
{
    public string type { get; set; }
    public List<Offers> flightOffers { get; set; }
}
