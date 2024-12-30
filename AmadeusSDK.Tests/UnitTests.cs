using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Moq;
using Moq.Protected;
using System.Net.Http.Json;
using AmadeusSDK.Tests;
using static AmadeusSDK.Models;
using AmadeusSDK;

namespace FlightsSystem.Services.Tests;

[TestClass]
public class UnitTests
{
    [TestMethod]
    public async Task SearchFlights()
    {
        var services = TestHelpers.BuildServiceCollection();

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
           .Protected()
           // Setup the PROTECTED method to mock
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.IsAny<HttpRequestMessage>(),
              ItExpr.IsAny<CancellationToken>()
           )
           // prepare the expected response of the mocked http call
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = JsonContent.Create(new FlightOffersResponse())
           })
           .Verifiable();

        // use real http client with mocked handler here
        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://test.com/"),
        };
        services.AddHttpClient<AmadeusClient>((serviceProvider, client) =>
        {
            client.BaseAddress = new Uri("https://test.api.amadeus.com/");
        });

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var amadeusClient = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await amadeusClient.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
