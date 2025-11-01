using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class ReportsPage
{
    private readonly DatabaseService _databaseService;
    private readonly PdfReportService _pdfReportService;
    private readonly CurrencyService _currencyService;

    public ReportsPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        _pdfReportService = new PdfReportService(_databaseService);
        _currencyService = new CurrencyService();
        PeriodPicker.SelectedIndexChanged += OnPeriodChanged;
    }
    
    private async Task<double> ConvertToRonAsync(double amount, string currency)
    {
        if (currency == "RON")
            return amount;
            
        var rates = await _currencyService.GetExchangeRatesAsync();
        
        if (rates.ContainsKey(currency) && rates[currency] > 0)
        {
            return amount / rates[currency];
        }
        
        return amount;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        PeriodPicker.SelectedIndex = 0;
        await LoadReport();
    }

    private async void OnPeriodChanged(object? sender, EventArgs e)
    {
        await LoadReport();
    }

    private async Task LoadReport()
    {
        try
        {
            var (startDate, endDate) = GetDateRange();
            var expenses = await _databaseService.GetExpensesByDateRangeAsync(startDate, endDate);

            if (!expenses.Any())
            {
                MonthlyTotalLabel.Text = "0.00 RON";
                MonthlyDetailsLabel.Text = "0 cheltuieli în perioada selectată";
                DailyAverageLabel.Text = "0.00 RON";
                MaxDayLabel.Text = "0.00 RON";
                TopCategoriesCollectionView.ItemsSource = null;
                return;
            }

            // Convert all amounts to RON
            var expensesInRon = new List<(Expense expense, double amountInRon)>();
            foreach (var expense in expenses)
            {
                var amountInRon = await ConvertToRonAsync(expense.Amount, expense.Currency);
                expensesInRon.Add((expense, amountInRon));
            }

            // Summary calculations using RON amounts
            var total = expensesInRon.Sum(e => e.amountInRon);
            var count = expenses.Count;
            var days = (endDate - startDate).Days + 1;
            var dailyAverage = total / days;

            // Daily statistics
            var dailyTotals = expensesInRon
                .GroupBy(e => e.expense.Date.Date)
                .Select(g => g.Sum(e => e.amountInRon))
                .OrderByDescending(x => x)
                .ToList();

            var maxDay = dailyTotals.FirstOrDefault();

            MonthlyTotalLabel.Text = $"{total:F2} RON";
            MonthlyDetailsLabel.Text = $"{count} cheltuieli în perioada selectată";
            DailyAverageLabel.Text = $"{dailyAverage:F2} RON";
            MaxDayLabel.Text = $"{maxDay:F2} RON";

            // Top categories using RON amounts
            var topCategories = expensesInRon
                .GroupBy(e => e.expense.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.amountInRon),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Amount)
                .Take(5)
                .Select(x => new CategoryReportViewModel
                {
                    DisplayText = $"{x.Category} ({x.Count})",
                    AmountText = $"{x.Amount:F2} RON"
                })
                .ToList();

            TopCategoriesCollectionView.ItemsSource = topCategories;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading report: {ex.Message}");
        }
    }

    private (DateTime, DateTime) GetDateRange()
    {
        var today = DateTime.Now;
        var endDate = today;
        DateTime startDate;

        return PeriodPicker.SelectedIndex switch
        {
            0 => (new DateTime(today.Year, today.Month, 1), today), // Current month
            1 => (new DateTime(today.Year, today.Month, 1).AddMonths(-1), new DateTime(today.Year, today.Month, 1).AddDays(-1)), // Last month
            2 => (today.AddMonths(-3), today), // Last 3 months
            3 => (today.AddMonths(-6), today), // Last 6 months
            4 => (today.AddYears(-1), today), // Last year
            _ => (new DateTime(today.Year, today.Month, 1), today)
        };
    }
}

public class CategoryReportViewModel
{
    public string DisplayText { get; set; } = string.Empty;
    public string AmountText { get; set; } = string.Empty;
}

