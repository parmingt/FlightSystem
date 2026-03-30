using FlightSystem.Data;
using FlightSystem.Services;
using FlightSystem.Services.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = FlightSystem.Services.Models;
using Data = FlightSystem.Data;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Testcontainers.PostgreSql;

namespace FlightsSystem.Services.Tests;

[TestClass]
public class BookingServiceTests
{
    private static ServiceProvider _serviceProvider;

    [ClassInitialize]
    public static async Task Initialize(TestContext testContext)
    {
        var serviceCollection = TestHelpers.BuildServiceCollection();
        var postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1").Build();
        await postgreSqlContainer.StartAsync();

        serviceCollection.AddDbContext<FlightContext>((_, optionsBuilder) =>
            optionsBuilder.UseNpgsql(postgreSqlContainer.GetConnectionString())
            , ServiceLifetime.Transient);

        _serviceProvider = serviceCollection.BuildServiceProvider();

        var context = _serviceProvider.GetRequiredService<FlightContext>();
        context.Database.EnsureCreated();

        context.Seats.ExecuteDelete();
        context.Segments.ExecuteDelete();
        context.Airports.AddRange([
            new Data.Airport() { Code = "JFK", Name = "Kennedy" },
            new Data.Airport() { Code = "LAX", Name = "Los Angeles" }
            ]);
        await context.SaveChangesAsync();
        context.Segments.Add(new Data.Segment()
        {
            Origin = context.Airports.First(a => a.Code == "JFK"),
            Destination = context.Airports.First(a => a.Code == "LAX"),
            CarrierCode = "AA",
            Number = "100",
            Departure = DateTime.UtcNow.AddDays(1),
            Seats = [new Seat()]
        });
        context.Bookings.RemoveRange(context.Bookings);
        await context.SaveChangesAsync();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
    }

    [TestMethod]
    public async Task PreventDoubleBooking()
    {
        var context = _serviceProvider.GetRequiredService<FlightContext>();
        try
        {
            var task1 = Task.Run(BookLaxFlight);
            var task2 = Task.Run(BookLaxFlight);
            await Task.WhenAll(task1, task2);
            Assert.Fail("Expected concurrency exception not thrown");
        }
        catch (DbUpdateConcurrencyException) { }

        var bookings = context.Bookings.Include(b => b.Price).Include(b => b.Seats).ToList();
        Assert.AreEqual(1, bookings.Count);
        Assert.AreEqual("fake_booking_id", bookings.First().BookingId);
    }

    [TestMethod]
    public async Task BookingReturnsBookingId()
    {
        var context = _serviceProvider.GetRequiredService<FlightContext>();
        var result = await BookLaxFlight();

        Assert.AreEqual("fake_booking_id", result.Data);
    }

    private Task<Result<string?>> BookLaxFlight()
    {
        var service = _serviceProvider.GetRequiredService<BookingService>();

        return service.BookFlight(new FlightOffer(
            DateTime.UtcNow,
            new Models.Price(300m, "USD", 300m),
            [
                new Models.Segment("AA", "100"
                        , new IataCode("JFK"), new IataCode("LAX"), DateTime.UtcNow.AddDays(1), "seg1")
            ],
            "",
            [],
            "",
            []
        ));
    }
}
