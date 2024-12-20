using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace FlightSystem.Data;

public class FlightContext : DbContext
{
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Segment> Segments { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    public FlightContext(DbContextOptions<FlightContext> options)
    : base(options)
    { }
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

public class Segment
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }

    public required Airport Origin { get; set; }
    public required Airport Destination { get; set; }
    public required DateTime Departure { get; set; }
}


public class Booking
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }

    public required List<Segment> Segments { get; set; } 
}