using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace FlightSystem.Data;

public class FlightContext : DbContext
{
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Airport> Airports { get; set; }

    public FlightContext(DbContextOptions<FlightContext> options)
    : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var atlanta = new Airport { Id = new Guid("C71F5D8D-0CB5-4543-A311-1634E9E7BFC6"), Code = "ATL", Name = "Atlanta" };
        var newark = new Airport { Id = new Guid("5654F336-9FAB-4C97-BE51-B6A14A51DCDA"), Code = "EWR", Name = "Newark" };
        modelBuilder.Entity<Airport>().HasData(
            atlanta,
            newark,
            new { Id = new Guid("C4AC12DF-AE7B-4B9E-99DC-79D3EC0FB6CF"), Code = "SLC", Name = "Salt Lake City" },
            new { Id = new Guid("A22F127A-05BB-4C62-9CDD-98B6470A536B"), Code = "PHI", Name = "Philadelphia" });

        modelBuilder.Entity<Flight>().HasData(
            new {
                Id = new Guid("F6CA3782-ECFD-4417-B588-B1D988FAF97B"),
                OriginId = newark.Id, 
                DestinationId = atlanta.Id, 
                Departure = DateTime.Now, 
                Duration = TimeSpan.FromHours(3) 
            });
    }
}

public class Flight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public required Airport Origin { get; set; }
    public required Airport Destination { get; set; }
    public DateTime Departure { get; set; }
    public TimeSpan Duration { get; set; }


}

[Index(nameof(Code), IsUnique = true)]
public class Airport
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
}