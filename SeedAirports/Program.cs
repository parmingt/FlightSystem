using CsvHelper;
using CsvHelper.Configuration.Attributes;
using FlightSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

var contextOptions = new DbContextOptionsBuilder<FlightContext>();
contextOptions.UseSqlite("Data Source=C:\\Users\\parmy\\Documents\\localflights.dat");
var context = new FlightContext(contextOptions.Options);
context.Database.EnsureCreated();

var airports = new List<Airport>();
using (var reader = new StreamReader("flat-ui__data-Thu Oct 03 2024.csv"))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
    var records = csv.GetRecords<CsvLine>();
    foreach (CsvLine item in records)
    {
        if (item.type == "large_airport" && item.iata_code != "null"){
            var existing = context.Airports.Where(a => a.Code == item.iata_code).Any();
            if (!existing)
            {
                context.Airports.Attach(new Airport()
                {
                    Code = item.iata_code,
                    Name = item.name
                });
            }
            else
            {
                Console.WriteLine(item.name);
            }
        }
    }
}
await context.SaveChangesAsync();

class CsvLine
{
    public string type { get; set; }
    public string name { get; set; }

    [Name("iata_code")]
    public string iata_code { get; set; }
}