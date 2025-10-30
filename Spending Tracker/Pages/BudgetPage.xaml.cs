using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class BudgetPage
{
    private readonly DatabaseService _databaseService;

    public BudgetPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategories();
        await LoadBudgets();
    }

    private async Task LoadCategories()
    {
        var categories = await _databaseService.GetCategoriesAsync();
        CategoryPicker.ItemsSource = categories.Select(c => c.Name).ToList();
    }

    private async void OnSaveBudgetClicked(object? sender, EventArgs e)
    {
        if (CategoryPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Eroare", "Selectează o categorie", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(BudgetEntry.Text) || !double.TryParse(BudgetEntry.Text, out double budgetAmount))
        {
            await DisplayAlert("Eroare", "Introduceți un buget valid", "OK");
            return;
        }

        var budget = new Budget
        {
            Category = CategoryPicker.SelectedItem.ToString()!,
            MonthlyLimit = budgetAmount,
            Month = DateTime.Now.Month,
            Year = DateTime.Now.Year
        };

        try
        {
            await _databaseService.SaveBudgetAsync(budget);
            BudgetEntry.Text = string.Empty;
            CategoryPicker.SelectedIndex = -1;
            await DisplayAlert("Succes", "Buget salvat", "OK");
            await LoadBudgets();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", $"Nu s-a putut salva: {ex.Message}", "OK");
        }
    }

    private async Task LoadBudgets()
    {
        try
        {
            var budgets = await _databaseService.GetBudgetsForMonthAsync(DateTime.Now.Month, DateTime.Now.Year);
            var expenses = await _databaseService.GetExpensesAsync();

            var budgetViewModels = budgets.Select(b =>
            {
                var spent = expenses
                    .Where(e => e.Category == b.Category && e.Date.Month == b.Month && e.Date.Year == b.Year)
                    .Sum(e => e.Amount);

                var progress = b.MonthlyLimit > 0 ? Math.Min(spent / b.MonthlyLimit, 1.0) : 0;
                var statusColor = progress >= 1.0 ? Colors.Red : (progress >= 0.8 ? Colors.Orange : Colors.Green);
                var progressColor = progress >= 1.0 ? Colors.Red : (progress >= 0.8 ? Colors.Orange : Colors.Green);

                return new BudgetViewModel
                {
                    Category = b.Category,
                    BudgetText = $"Buget: {b.MonthlyLimit:F2} RON",
                    SpentText = $"Cheltuit: {spent:F2} RON",
                    Progress = progress,
                    ProgressColor = progressColor,
                    StatusColor = statusColor,
                    PercentageText = $"{(progress * 100):F1}% din buget utilizat"
                };
            }).ToList();

            BudgetsCollectionView.ItemsSource = budgetViewModels;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading budgets: {ex.Message}");
        }
    }
}

public class BudgetViewModel
{
    public string Category { get; set; } = string.Empty;
    public string BudgetText { get; set; } = string.Empty;
    public string SpentText { get; set; } = string.Empty;
    public double Progress { get; set; }
    public Color ProgressColor { get; set; }
    public Color StatusColor { get; set; }
    public string PercentageText { get; set; } = string.Empty;
}

