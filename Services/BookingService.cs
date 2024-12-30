using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Models.Booking>> GetBookings()
    {
        var bookings = await context.Bookings.ToListAsync();
        return bookings.Select(b => 
            new Models.Booking(
                b.Segments.Select(s => 
                    new Models.Segment("test", "11"
                    , new IataCode(s.Origin.Code)
                    , new IataCode(s.Destination.Code), s.Departure)).ToList()))
            .ToList();
    }
}
