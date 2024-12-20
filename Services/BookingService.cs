using FlightSystem.Data;
using FlightSystem.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightSystem.Services;

public class BookingService
{
    private FlightContext context;

    public BookingService(FlightContext context)
    {
        this.context = context;
    }
    public async Task BookFlight(Flight selectedFlight)
    {
        var newBooking = new Data.Booking()
        {
            Segments = selectedFlight.Segments.Select(s => new Data.Segment
            {
                Origin = context.Airports.First(a => a.Code == s.Origin.ToString()),
                Destination = context.Airports.First(a => a.Code == s.Destination.ToString()),
                Departure = s.Departure
            }).ToList()
        };
        context.Bookings.Add(newBooking);
        await context.SaveChangesAsync();
    }
}
