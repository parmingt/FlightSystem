using FlightSystem.Data;
using FlightSystem.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace FlightsSystem.Services.Tests;

[TestClass]
public class AirportsServiceTests
{
    private static SqliteConnection _connection;
    private static ServiceProvider _serviceProvider;

    [ClassInitialize]
    public static void Initialize(TestContext testContext)
    {
        // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
        // at the end of the test (see Dispose below).
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var dbContextOptions = new DbContextOptionsBuilder<FlightContext>()
        .UseSqlite(_connection)
            .Options;

        // Create the schema and seed some data
        using var context = new FlightContext(dbContextOptions);
        context.Database.EnsureCreated();
        context.Airports.AddRange(new List<Airport> {
            new Airport(){Code = "EWR", Name = "Newark" }
        });
        context.SaveChanges();

        var serviceCollection = TestHelpers.BuildServiceCollection();
        serviceCollection.AddDbContext<FlightContext>((_, optionsBuilder) =>
            optionsBuilder.UseSqlite(_connection));
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [ClassCleanup]
    public static void Cleanup()
    {
        _connection.Dispose();
    }

    [TestMethod]
    public async Task GetAirport()
    {
        var service = _serviceProvider.GetRequiredService<AirportsService>();
        var airports = await service.SearchAirportsAsync("EWR");
        Assert.IsTrue(airports.Count() == 1);
    }
}
