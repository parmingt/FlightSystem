using FlightSystem.Data;
using FlightSystem.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace FlightsSystem.Services.Tests;

[TestClass]
public class AirportsServiceTests
{
    private static SqliteConnection _connection;
    private static ServiceProvider _serviceProvider;

    [ClassInitialize]
    public static async Task Initialize(TestContext testContext)
    {
        var postgreSqlContainer = new PostgreSqlBuilder("postgres:15.1").Build();
        await postgreSqlContainer.StartAsync();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var dbContextOptions = new DbContextOptionsBuilder<FlightContext>()
            .UseNpgsql(postgreSqlContainer.GetConnectionString())
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
            optionsBuilder.UseNpgsql(postgreSqlContainer.GetConnectionString())
            , ServiceLifetime.Transient);
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [ClassCleanup]
    public static void Cleanup()
    {
    }

    [TestMethod]
    public async Task GetAirport()
    {
        var service = _serviceProvider.GetRequiredService<AirportsService>();
        var airports = await service.SearchAirportsAsync("EWR");
        Assert.IsTrue(airports.Count() == 1);
    }
}
