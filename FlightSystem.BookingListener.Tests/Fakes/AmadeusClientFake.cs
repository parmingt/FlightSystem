using AmadeusSDK;
using AmadeusSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.BookingListener.Tests.Fakes;

internal class AmadeusClientFake : IAmadeusClient
{
    public Task<List<OffersSearch.Offers>> BookFlight(FlightOrder order)
    {
        return Task.FromResult(order.flightOffers);
    }

    public Task<List<OffersSearch.Offers>> SearchFlightsAsync(string origin, string destination, DateTime departure, int numAdults = 1)
    {
        throw new NotImplementedException();
    }
}
