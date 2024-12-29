namespace AmadeusSDK;

public class AmadeusClientOptions
{
    public AmadeusClientOptions(string clientId, string clientSecret)
    {
        ClientId = clientId;
        ClientSecret = clientSecret;
    }

    public string ClientId { get; set; }
    public string ClientSecret {  get; set; }
}
