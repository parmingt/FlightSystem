using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace FlightSystem.Data;

public class FlightContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Airport> Airports { get; set; }

    public FlightContext(DbContextOptions<FlightContext> options)
    : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=localflights.dat");
    }
}

public class Flight
{
    public Guid Id { get; set; }
    public Airport Origin { get; set; }
    public Airport Destination { get; set; }
    public DateTime Departure { get; set; }
    public TimeSpan Duration { get; set; }


}

public class Airport
{
    public Guid Id { get; set; }
    public string Code { get; set; }
}