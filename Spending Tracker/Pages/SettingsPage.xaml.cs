using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class SettingsPage
{
    private readonly DatabaseService _databaseService;

    public SettingsPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadData();
    }

    private async Task LoadData()
    {
        // Load categories for picker
        var categories = await _databaseService.GetCategoriesAsync();
        BudgetCategoryPicker.ItemsSource = categories.Select(c => c.Name).ToList();

        // Load budgets
        var budgets = await _databaseService.GetBudgetsAsync();
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;
        
        var currentBudgets = budgets
            .Where(b => b.Month == currentMonth && b.Year == currentYear)
            .Select(b => new SettingsBudgetViewModel
            {
                CategoryName = b.Category,
                LimitDisplay = $"{b.MonthlyLimit:F2} RON"
            })
            .ToList();

        BudgetsCollectionView.ItemsSource = currentBudgets;

        // Load statistics
        var expenses = await _databaseService.GetExpensesAsync();
        TotalExpensesCountLabel.Text = expenses.Count.ToString();
        CategoriesCountLabel.Text = categories.Count.ToString();
    }

    private async void OnSaveBudgetClicked(object? sender, EventArgs e)
    {
        if (BudgetCategoryPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Eroare", "Selectează o categorie", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(BudgetAmountEntry.Text) || 
            !double.TryParse(BudgetAmountEntry.Text, out double amount))
        {
            await DisplayAlert("Eroare", "Introdu o sumă validă", "OK");
            return;
        }

        var category = BudgetCategoryPicker.SelectedItem.ToString()!;
        var currentMonth = DateTime.Now.Month;
        var currentYear = DateTime.Now.Year;

        var budget = new Budget
        {
            Category = category,
            MonthlyLimit = amount,
            Month = currentMonth,
            Year = currentYear
        };

        await _databaseService.SaveBudgetAsync(budget);
        BudgetAmountEntry.Text = string.Empty;
        BudgetCategoryPicker.SelectedIndex = -1;
        
        await DisplayAlert("Succes", "Buget salvat cu succes!", "OK");
        await LoadData();
    }

    private async void OnClearDataClicked(object? sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Atenție", 
            "Ești sigur că vrei să ștergi toate cheltuielile? Această acțiune nu poate fi anulată!", 
            "Da, șterge tot", "Anulează");

        if (confirm)
        {
            var expenses = await _databaseService.GetExpensesAsync();
            foreach (var expense in expenses)
            {
                await _databaseService.DeleteExpenseAsync(expense);
            }

            await DisplayAlert("Succes", "Toate cheltuielile au fost șterse", "OK");
            await LoadData();
        }
    }
}

public class SettingsBudgetViewModel
{
    public string CategoryName { get; set; } = string.Empty;
    public string LimitDisplay { get; set; } = string.Empty;
}
