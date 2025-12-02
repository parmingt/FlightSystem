using FlightSystem.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FlightContext>(options =>
    //options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]),
    options.UseNpgsql(builder.Configuration.GetConnectionString("FlightsDb")),
    ServiceLifetime.Scoped);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/bookflight", (HttpContext httpContext, BookFlightRequest request, FlightContext context) =>
{
    //var booking = new Flight
    //{
    //    Origin = context.Airports.First(a => a.Code == request.origin),
    //    Destination = context.Airports.First(a => a.Code == request.destination),
    //    Departure = request.date
    //};
    //context.Flights.Add(booking);
    //context.SaveChanges();

    return "Booked";
})
.WithName("BookFlight")
.WithOpenApi();

app.MapGet("/airports", (FlightContext context) =>
{
    return context.Airports.ToList();
});

app.Run();

internal record BookFlightRequest(DateTime date, string origin, string destination);
