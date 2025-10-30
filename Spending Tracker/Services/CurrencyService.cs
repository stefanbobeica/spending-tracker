using Newtonsoft.Json;

namespace Spending_Tracker.Services;

public class CurrencyService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.exchangerate-api.com/v4/latest/RON";

    public CurrencyService()
    {
        _httpClient = new HttpClient();
    }

    public async Task<Dictionary<string, double>> GetExchangeRatesAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(ApiUrl);
            var data = JsonConvert.DeserializeObject<ExchangeRateResponse>(response);
            return data?.Rates ?? new Dictionary<string, double>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching rates: {ex.Message}");
            // Return default rates if API fails
            return new Dictionary<string, double>
            {
                { "RON", 1.0 },
                { "EUR", 0.20 },
                { "USD", 0.22 },
                { "GBP", 0.17 }
            };
        }
    }

    public async Task<double> ConvertCurrencyAsync(double amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency)
            return amount;

        var rates = await GetExchangeRatesAsync();
        
        if (rates.ContainsKey(toCurrency))
        {
            return amount * rates[toCurrency];
        }
        
        return amount;
    }

    private class ExchangeRateResponse
    {
        [JsonProperty("rates")]
        public Dictionary<string, double>? Rates { get; set; }
    }
}

