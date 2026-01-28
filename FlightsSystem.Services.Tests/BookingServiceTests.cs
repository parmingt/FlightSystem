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

namespace FlightsSystem.Services.Tests;

[TestClass]
public class BookingServiceTests
{
    private static ServiceProvider _serviceProvider;

    [ClassInitialize]
    public static void Initialize(TestContext testContext)
    {

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var dbContextOptions = new DbContextOptionsBuilder<FlightContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=Flights;User ID=postgres;password=postgres;timeout=1000");

        // Create the schema and seed some data
        using var context = new FlightContext(dbContextOptions.Options);
        context.Database.EnsureCreated();
        context.Seats.ExecuteDelete();
        context.Segments.ExecuteDelete();
        context.Segments.Add(new Data.Segment()
        {
            Origin = context.Airports.First(a => a.Code == "JFK"),
            Destination = context.Airports.First(a => a.Code == "LAX"),
            CarrierCode = "AA",
            Number = "100",
            Departure = DateTime.UtcNow.AddDays(1),
            Seats = [new Seat() { Version = 1 }]
        });
        context.SaveChanges();

        var serviceCollection = TestHelpers.BuildServiceCollection();
        serviceCollection.AddDbContext<FlightContext>((_, optionsBuilder) =>
            optionsBuilder.UseNpgsql("Server=localhost;Port=5432;Database=Flights;User ID=postgres;password=postgres;timeout=1000")
            , ServiceLifetime.Transient);
        serviceCollection.AddScoped<AirportsService>();
        serviceCollection.AddScoped<FlightSearchService>();
        serviceCollection.AddTransient<BookingService>();
        serviceCollection.AddMemoryCache();
        serviceCollection.AddTransient((serviceProvider) =>
            FakeAmadeusClient.CreateFakeClient(serviceProvider.GetRequiredService<IMemoryCache>()));

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [TestInitialize]
    public void TestInitialize()
    {
        var context = _serviceProvider.GetRequiredService<FlightContext>();
        context.Seats.ToList().ForEach(s => s.Booking = null);
        context.Bookings.RemoveRange(context.Bookings);
        context.SaveChanges();
    }

    [TestMethod]
    public async Task PreventDoubleBooking()
    {
        var context = _serviceProvider.GetRequiredService<FlightContext>();
        try
        {
            var task1 = Task.Run(bookLaxFlight);
            var task2 = Task.Run(bookLaxFlight);
            await Task.WhenAll(task1, task2);
            Assert.Fail("Expected concurrency exception not thrown");
        }
        catch (DbUpdateConcurrencyException) { }

        var bookings = context.Bookings.Include(b => b.Price).ToList();
        Assert.AreEqual(1, bookings.Count);

        async Task bookLaxFlight()
        {
            var service = _serviceProvider.GetRequiredService<BookingService>();

            await service.BookFlight(new FlightOffer(
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
}
