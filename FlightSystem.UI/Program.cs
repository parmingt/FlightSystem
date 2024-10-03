using FlightSystem.Data;
using FlightSystem.Services;
using FlightSystem.UI;
using FlightSystem.UI.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBlazorBootstrap();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContextFactory<FlightContext>(options =>
    options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]), ServiceLifetime.Singleton);

builder.Services.AddScoped<AirportsService>();
builder.Services.AddScoped<FlightSearchService>();

builder.Services.AddHttpClient<IRoutesClient, AmadeusClient>((serviceProvider, client) =>
{
    var token = builder.Configuration["amadeusToken"];
    client.BaseAddress = new Uri("https://test.api.amadeus.com/v2/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
