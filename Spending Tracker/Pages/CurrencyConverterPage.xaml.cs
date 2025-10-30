using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class CurrencyConverterPage : ContentPage
{
    private readonly CurrencyService _currencyService;
    private Dictionary<string, double> _exchangeRates = new();

    public CurrencyConverterPage()
    {
        InitializeComponent();
        _currencyService = new CurrencyService();
        
        FromCurrencyPicker.SelectedIndex = 0; // RON
        ToCurrencyPicker.SelectedIndex = 1; // EUR
    }

    private void OnAmountChanged(object? sender, TextChangedEventArgs e)
    {
        // Auto-convert if rates are loaded
        if (_exchangeRates.Any() && !string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            PerformConversion();
        }
    }

    private void OnCurrencyChanged(object? sender, EventArgs e)
    {
        // Auto-convert if rates are loaded and amount is entered
        if (_exchangeRates.Any() && !string.IsNullOrWhiteSpace(AmountEntry.Text))
        {
            PerformConversion();
        }
    }

    private void OnSwapClicked(object? sender, EventArgs e)
    {
        var tempIndex = FromCurrencyPicker.SelectedIndex;
        FromCurrencyPicker.SelectedIndex = ToCurrencyPicker.SelectedIndex;
        ToCurrencyPicker.SelectedIndex = tempIndex;
    }

    private async void OnConvertClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(AmountEntry.Text))
        {
            await DisplayAlert("Eroare", "Introdu o sumă pentru conversie", "OK");
            return;
        }

        if (FromCurrencyPicker.SelectedIndex < 0 || ToCurrencyPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Eroare", "Selectează ambele monede", "OK");
            return;
        }

        LoadingIndicator.IsRunning = true;
        InfoLabel.Text = "Se încarcă ratele de schimb...";

        try
        {
            _exchangeRates = await _currencyService.GetExchangeRatesAsync();
            PerformConversion();
            InfoLabel.Text = $"Actualizat: {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", $"Nu s-au putut încărca ratele: {ex.Message}", "OK");
            InfoLabel.Text = "Eroare la încărcarea ratelor";
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
        }
    }

    private void PerformConversion()
    {
        if (!double.TryParse(AmountEntry.Text, out double amount))
        {
            ResultLabel.Text = "0.00";
            RateLabel.Text = "Rată: -";
            return;
        }

        var fromCurrency = FromCurrencyPicker.SelectedItem?.ToString() ?? "RON";
        var toCurrency = ToCurrencyPicker.SelectedItem?.ToString() ?? "EUR";

        if (fromCurrency == toCurrency)
        {
            ResultLabel.Text = $"{amount:F2}";
            RateLabel.Text = "Rată: 1.00";
            return;
        }

        // Convert: amount in RON * rate = amount in target currency
        // If from is not RON, first convert to RON, then to target
        double result = amount;
        double rate = 1.0;

        if (_exchangeRates.ContainsKey(toCurrency))
        {
            if (fromCurrency == "RON")
            {
                result = amount * _exchangeRates[toCurrency];
                rate = _exchangeRates[toCurrency];
            }
            else if (_exchangeRates.ContainsKey(fromCurrency))
            {
                // Convert from source to RON, then RON to target
                double amountInRon = amount / _exchangeRates[fromCurrency];
                result = amountInRon * _exchangeRates[toCurrency];
                rate = _exchangeRates[toCurrency] / _exchangeRates[fromCurrency];
            }
        }

        ResultLabel.Text = $"{result:F2}";
        RateLabel.Text = $"Rată: 1 {fromCurrency} = {rate:F4} {toCurrency}";
    }
}
