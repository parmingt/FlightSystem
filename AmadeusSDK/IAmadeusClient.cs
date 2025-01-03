namespace AmadeusSDK;

public interface IAmadeusClient
{
    Task<string> GetTokenAsync();
    Task<List<Models.OffersSearch>> SearchFlightsAsync(string origin, string destination, DateTime departure, int numAdults = 1);
}