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

        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        // mock for general http call
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
               Content = JsonContent.Create(new FlightOffersResponse()
               {
                   data = new Offers[] { new Offers() }
               })
           })
           .Verifiable();

        // mock for token call
        handlerMock
           .Protected()
           // Setup the PROTECTED method to mock
           .Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(m => m.Content is FormUrlEncodedContent),
              ItExpr.IsAny<CancellationToken>()
           )
           // prepare the expected response of the mocked http call
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = JsonContent.Create(new AmadeusToken()
               {
                   expires_in = 20,
                   access_token = "123"
               })
           })
           .Verifiable();

        var services = new ServiceCollection();
        services.AddAmadeusClient("", "");

        // Replace httpclient with mock
        services.AddHttpClient<AmadeusClient>()
            .ConfigurePrimaryHttpMessageHandler(() => handlerMock.Object);

        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var amadeusClient = serviceProvider.GetRequiredService<AmadeusClient>();

        var origin = "EWR";
        var destination = "SLC";

        var flights = await amadeusClient.SearchFlightsAsync(origin, destination, DateTime.Now);

        Assert.IsTrue(flights.Any());
    }
}
