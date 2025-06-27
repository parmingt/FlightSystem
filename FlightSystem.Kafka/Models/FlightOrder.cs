using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Kafka.Models;

public class FlightOrder
{
    public OffersSearch.Offers[] flightOffers { get; set; }
}
