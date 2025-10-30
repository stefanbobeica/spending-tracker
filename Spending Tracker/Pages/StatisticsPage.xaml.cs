using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class StatisticsPage
{
    private readonly DatabaseService _databaseService;

    public StatisticsPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        StartDatePicker.Date = startOfMonth;
        EndDatePicker.Date = DateTime.Now;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadStatistics();
    }

    private async void OnDateChanged(object? sender, DateChangedEventArgs e)
    {
        await LoadStatistics();
    }

    private async Task LoadStatistics()
    {
        try
        {
            var expenses = await _databaseService.GetExpensesByDateRangeAsync(
                StartDatePicker.Date,
                EndDatePicker.Date.AddDays(1).AddSeconds(-1)
            );

            if (!expenses.Any())
            {
                TotalExpensesLabel.Text = "0.00 RON";
                CountLabel.Text = "0";
                AverageLabel.Text = "0.00 RON";
                MaxDayLabel.Text = "0.00";
                CategoriesCountLabel.Text = "0";
                PeriodLabel.Text = "Nicio cheltuiala";
                BarChartCollectionView.ItemsSource = null;
                CategoryCollectionView.ItemsSource = null;
                ChartStatusLabel.Text = "Nicio data disponibila";
                return;
            }

            // Summary calculations
            var total = expenses.Sum(e => e.Amount);
            var count = expenses.Count;
            var days = (EndDatePicker.Date - StartDatePicker.Date).Days + 1;
            var average = total / days;

            // Calculate max day
            var dailyTotals = expenses
                .GroupBy(e => e.Date.Date)
                .Select(g => g.Sum(e => e.Amount))
                .OrderByDescending(x => x)
                .FirstOrDefault();

            TotalExpensesLabel.Text = $"{total:F2} RON";
            CountLabel.Text = count.ToString();
            AverageLabel.Text = $"{average:F2} RON";
            MaxDayLabel.Text = $"{dailyTotals:F2}";
            PeriodLabel.Text = $"{days} zile";

            // Get categories with colors
            var categories = await _databaseService.GetCategoriesAsync();
            var categoryColors = categories.ToDictionary(c => c.Name, c => c.Color);
            
            CategoriesCountLabel.Text = categories.Count.ToString();

            // Group by category
            var categoryGroups = expenses
                .GroupBy(e => e.Category)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = g.Sum(e => e.Amount),
                    Color = categoryColors.ContainsKey(g.Key) ? categoryColors[g.Key] : "#999999"
                })
                .OrderByDescending(x => x.Amount)
                .ToList();

            // Calculate max amount for progress bar scaling
            var maxAmount = categoryGroups.Max(g => g.Amount);

            // Category breakdown for bar chart and detailed view
            var categoryBreakdown = categoryGroups.Select(g => new CategoryBreakdownViewModel
            {
                Category = g.Category,
                Amount = $"{g.Amount:F2} RON",
                Color = Color.FromArgb(g.Color),
                Progress = maxAmount > 0 ? g.Amount / maxAmount : 0,
                PercentageText = $"{(g.Amount / total * 100):F1}% din total"
            }).ToList();

            // Set bar chart data
            BarChartCollectionView.ItemsSource = categoryBreakdown;
            ChartStatusLabel.Text = $"Distribuție {categoryGroups.Count} categorii";

            // Also set detailed category breakdown
            CategoryCollectionView.ItemsSource = categoryBreakdown;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading statistics: {ex.Message}");
            ChartStatusLabel.Text = "Eroare la încărcarea datelor";
        }
    }
}

public class CategoryBreakdownViewModel
{
    public string Category { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public Color Color { get; set; } = Colors.Gray;
    public double Progress { get; set; }
    public string PercentageText { get; set; } = string.Empty;
}

