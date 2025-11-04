using AmadeusSDK;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using FlightSystem.Data;
using FlightSystem.Services;
using FlightSystem.UI.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using FlightSystem.Kafka.Models;
using FlightSystem.Services.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddBlazorBootstrap();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMemoryCache();

builder.Services.AddDbContextFactory<FlightContext>(options =>
    options.UseSqlite(builder.Configuration["ConnectionStrings:SQLiteDefault"]), ServiceLifetime.Singleton);

builder.Services.AddAmadeusClient(builder.Configuration["Amadeus:ClientId"], builder.Configuration["Amadeus:ClientSecret"]);

builder.Services.AddScoped<AirportsService>();
builder.Services.AddScoped<FlightSearchService>();
builder.Services.AddScoped<BookingService>();

builder.Services.Configure<SchemaRegistryConfig>(builder.Configuration.GetSection("SchemaRegistry"));
builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var config = sp.GetRequiredService<IOptions<SchemaRegistryConfig>>();
    return new CachedSchemaRegistryClient(config.Value);
});
builder.Services.AddSingleton(services =>
{
    var config = new ProducerConfig
    {
        BootstrapServers = builder.Configuration["Kafka:BootstrapServers"]
    };

    var schemaRegistry = services.GetRequiredService<ISchemaRegistryClient>();
    return new ProducerBuilder<string, FlightOrder>(config)
        .SetValueSerializer(new JsonSerializer<FlightOrder>(schemaRegistry)).Build();
});

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.WriteTo.File("C:\\log\\Flights\\apptest.log", rollingInterval: RollingInterval.Day);
    configuration.Enrich.FromLogContext();
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
