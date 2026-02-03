using FlightSystem.Data;
using FlightSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

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
    public async Task BookFlight(FlightOffer selectedFlight)
    {
        var confirmed = await flightSearchService.ConfirmFlight(selectedFlight);

        if (!confirmed)
        {
            Console.WriteLine("outdated flight");
            return;
        }

        // Create segments in db
        var segments = selectedFlight.Segments.Select(segment =>
            context.Segments.Include(s => s.Seats).FirstOrDefault(s =>
                segment.CarrierCode == s.CarrierCode &&
                segment.Number == s.Number &&
                segment.Departure.Date == s.Departure.Date))
            .Where(s => s is not null).ToList();

        var segmentsToCreate = selectedFlight.Segments
            .Where(s => !segments.Any(es =>
                es.CarrierCode == s.CarrierCode &&
                es.Number == s.Number &&
                es.Departure.Date == s.Departure.Date))
            .Select(s => new Data.Segment
        {
            CarrierCode = s.CarrierCode,
            Number = s.Number,
            Origin = context.Airports.First(a => a.Code == s.Origin.ToString()),
            Destination = context.Airports.First(a => a.Code == s.Destination.ToString()),
            Departure = s.Departure,
            Seats = [
                new Data.Seat()
                {
                    Version = 1
                }]
        }).ToList();
        context.Segments.AddRange(segmentsToCreate);

        var newBooking = new Data.Booking()
        {
            Price = new Data.Price()
            {
                Total = selectedFlight.Price.Total,
                Currency = context.Currency.First(c => c.Name == selectedFlight.Price.Currency)
            },
            Status = context.BookingStatus.First(s => s.Name == "Pending"),
            BookingDate = DateTime.UtcNow
        };
        context.Bookings.Add(newBooking);
        foreach (var seat in segments.SelectMany(s => s.Seats))
        {
            seat.Booking = newBooking;
            seat.Version++;
        }
        await context.SaveChangesAsync();

        var booking = await flightSearchService.BookFlight(selectedFlight);

        if (booking is null)
            return;

        newBooking.BookingId = booking.BookingId;
        await context.SaveChangesAsync();
    }

    public async Task<List<BookedFlight>> GetBookings()
    {
        var bookings = await context.Bookings
            //.Include(b => b.Segments).ThenInclude(s => s.Origin)
            //.Include(b => b.Segments).ThenInclude(s => s.Destination)
            //.Include(b => b.Price).ThenInclude(p => p.Currency)
        .ToListAsync();

        if (bookings is null)
            return new();

        return [];
        //return bookings.Select(b => 
        //    new BookedFlight(new FlightOffer(
        //        b.Segments.First().Departure,
        //        new Models.Price(b.Price.Total, b.Price.Currency.Name, 0),
        //        b.Segments.Select(s => 
        //            new Models.Segment("test", "11"
        //                , new IataCode(s.Origin.Code)
        //                , new IataCode(s.Destination.Code)
        //                , s.Departure
        //                , "")
        //            ).ToList() 
        //        , "", [], "", []), b.BookingDate))
        //    .ToList();
    }
}
