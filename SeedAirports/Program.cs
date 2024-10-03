using FlightSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine(File.Exists("../localflights.dat"));
var contextOptions = new DbContextOptionsBuilder<FlightContext>();
contextOptions.UseSqlite("Data Source=../localflights.dat");
var context = new FlightContext(contextOptions.Options);
context.Database.EnsureCreated();
Console.WriteLine(context.Airports.Count());