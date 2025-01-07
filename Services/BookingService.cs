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
    private readonly FlightSearchService flightSearchService;

    public BookingService(FlightContext context, FlightSearchService flightSearchService)
    {
        this.context = context;
        this.flightSearchService = flightSearchService;
    }
    public async Task BookFlight(Flight selectedFlight)
    {
        var confirmed = await flightSearchService.ConfirmFlight(selectedFlight);

        if (!confirmed)
        {
            Console.WriteLine("outdated flight");
            return;
        }

        var newBooking = new Data.Booking()
        {
            Segments = selectedFlight.Segments.Select(s => new Data.Segment
            {
                Origin = context.Airports.First(a => a.Code == s.Origin.ToString()),
                Destination = context.Airports.First(a => a.Code == s.Destination.ToString()),
                Departure = s.Departure
            }).ToList(),
            Price = new Data.Price()
            {
                Total = selectedFlight.Price.Total,
                Currency = context.Currency.First(c => c.Name == selectedFlight.Price.Currency)
            },
            Status = context.BookingStatus.First(s => s.Name == "Pending")
        };
        context.Bookings.Add(newBooking);
        await context.SaveChangesAsync();
    }

    public async Task<List<Flight>> GetBookings()
    {
        var bookings = await context.Bookings
            .Include(b => b.Segments).ThenInclude(s => s.Origin)
            .Include(b => b.Segments).ThenInclude(s => s.Destination)
            .Include(b => b.Price).ThenInclude(p => p.Currency)
        .ToListAsync();

        if (bookings is null)
            return new();

        return bookings.Select(b => 
            new Flight(
                b.Segments.First().Departure,
                new Models.Price(b.Price.Total, b.Price.Currency.Name),
                b.Segments.Select(s => 
                    new Models.Segment("test", "11"
                        , new IataCode(s.Origin.Code)
                        , new IataCode(s.Destination.Code), s.Departure)
                    ).ToList() 
                , ""))
            .ToList();
    }
}
