﻿using Microsoft.EntityFrameworkCore;
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