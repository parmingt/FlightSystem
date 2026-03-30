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
    public async Task<Result<string?>> BookFlight(FlightOffer selectedFlight)
    {
        var confirmed = await flightSearchService.ConfirmFlight(selectedFlight);

        if (!confirmed)
        {
            Console.WriteLine("outdated flight");
            return new Failure("Outdated flight");
        }

        // Create segments in db
        var segments = selectedFlight.Segments.Select(segment =>
            context.Segments.Include(s => s.Seats).FirstOrDefault(s =>
                s.Seats.Any(s => s.Booking == null) &&
                segment.CarrierCode == s.CarrierCode &&
                segment.Number == s.Number &&
                segment.Departure.Date == s.Departure.Date))
            .Where(s => s is { Seats.Count: >0 });

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
            BookingDate = DateTime.UtcNow,
            Seats = segments.Concat(segmentsToCreate).Select(s => s.Seats.First(s => s.Booking == null)).ToList()
        };
        context.Bookings.Add(newBooking);
        foreach (var seat in newBooking.Seats)
        {
            seat.Booking = newBooking;
        }
        await context.SaveChangesAsync();

        var bookingResult = await flightSearchService.BookFlight(selectedFlight);

        if (!bookingResult.Success)
        {
            context.Bookings.Remove(newBooking);
            return new Failure(bookingResult.ErrorMessage);
        }

        newBooking.BookingId = bookingResult.Data.BookingId;
        await context.SaveChangesAsync();
        return Result<string?>.SuccessResult(bookingResult.Data.BookingId);
    }

    public async Task<List<Models.BookedFlightSummary>> GetBookings()
    {
        var bookings = await context.Bookings
            .Where(b => b.Seats.Any())
            .Include(b => b.Seats).ThenInclude(s => s.Segment).ThenInclude(s => s.Origin)
            .Include(b => b.Seats).ThenInclude(s => s.Segment).ThenInclude(s => s.Destination)
            .Include(b => b.Price).ThenInclude(p => p.Currency)
            .Select(b => new Models.BookedFlightSummary(b.BookingDate, b.BookingId ?? ""
                , new IataCode(b.Seats.OrderBy(s => s.Segment.Departure).First().Segment.Origin.Code)
                , new IataCode(b.Seats.OrderBy(s => s.Segment.Departure).Last().Segment.Destination.Code)
                , new Models.Price(b.Price.Total, b.Price.Currency.Name, b.Price.Total)))
        .ToListAsync();

        return bookings;
    }
}
