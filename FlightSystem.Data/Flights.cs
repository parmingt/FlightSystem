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
    public DbSet<Flight> Flights { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<BookingStatus> BookingStatus { get; set; }

    public FlightContext(DbContextOptions<FlightContext> options)
    : base(options)
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookingStatus>().HasData([
                new() {Id=1,Name="Pending"},
                new() {Id=2,Name="Confirmed"}
            ]);

        modelBuilder.Entity<Currency>().HasData([
            new() {Name = "USD", Id = Guid.NewGuid() },
            new() {Name = "EUR", Id = Guid.NewGuid() }
            ]);

        // guid identities
        foreach (var entity in modelBuilder.Model.GetEntityTypes()
            .Where(t =>
                t.ClrType.GetProperties()
                    .Any(p => p.CustomAttributes.Any(a => a.AttributeType == typeof(DatabaseGeneratedAttribute)))))
        {
            foreach (var property in entity.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(Guid) && p.CustomAttributes.Any(a => a.AttributeType == typeof(DatabaseGeneratedAttribute))))
            {
                modelBuilder
                    .Entity(entity.ClrType)
                    .Property(property.Name)
                    .HasDefaultValueSql("newsequentialid()");
            }
        }

        modelBuilder.Entity<Seat>()
            .Property(s => s.Version)
            .IsConcurrencyToken();
    }
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
    public string CarrierCode { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    public required Airport Origin { get; set; }
    public required Airport Destination { get; set; }
    public required DateTime Departure { get; set; }
    public required List<Seat> Seats { get; set; }
}

public class Flight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public required List<Segment> Segments { get; set; }
}

public class Seat
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public int Version { get; set; }
    public Booking? Booking { get; set; }
}

    public class Booking
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }

    public required Price Price { get; set; }

    public required BookingStatus Status { get; set; }
    public required DateTime BookingDate { get; set; } = DateTime.MinValue;
}

public class Price
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public Guid BookingId { get; set; }
    public decimal Total { get; set; }
    public required Currency Currency { get; set; }
}

public class Currency
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public Guid? Id { get; set; }
    public required string Name { get; set; } 
}

public class BookingStatus
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }
}