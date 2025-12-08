using AmadeusSDK;
using Moq.Protected;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace FlightsSystem.Services.Tests;

internal static class FakeAmadeusClient
{
    public static AmadeusClient CreateFakeClient(IMemoryCache cache)
    {
        return new AmadeusClient(cache, CreateHttpClient(), new AmadeusClientOptions("", ""));
    }

    private static HttpClient CreateHttpClient()
    {
        var messageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        messageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .Returns(async (HttpRequestMessage request, CancellationToken token) =>
            {
                HttpResponseMessage response = new HttpResponseMessage();
                var requestMessageContent = await request.Content.ReadAsStringAsync();

                if (request.RequestUri.ToString().Contains("v1/shopping/flight-offers/pricing") && request.Method == HttpMethod.Post)
                    response.Content = request.Content;
                else if (request.RequestUri.ToString().Contains("v1/security/oauth2/token") && request.Method == HttpMethod.Post)
                    response.Content = new StringContent("{\"access_token\":\"fake_token\",\"token_type\":\"Bearer\",\"expires_in\":1799,\"state\":\"approved\"}", Encoding.UTF8, "application/json");

                return response;
            })
            .Verifiable();

        var httpClient = new HttpClient(messageHandlerMock.Object);
        httpClient.BaseAddress = new Uri("https://test.amadeus.com/");
        return httpClient;
    }
}
