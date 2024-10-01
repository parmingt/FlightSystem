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
}

public class Flight
{
    public Guid Id { get; set; }
    public required Airport Origin { get; set; }
    public required Airport Destination { get; set; }
    public DateTime Departure { get; set; }
    public TimeSpan Duration { get; set; }


}

[Index(nameof(Code), IsUnique = true)]
public class Airport
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}