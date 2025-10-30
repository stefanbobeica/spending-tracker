using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class ExpenseListPage
{
    private readonly DatabaseService _databaseService;
    private List<Expense> _allExpenses = new();

    public ExpenseListPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        FilterPicker.SelectedIndexChanged += OnFilterChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategories();
        await LoadExpenses();
    }

    private async Task LoadCategories()
    {
        var categories = await _databaseService.GetCategoriesAsync();
        var categoryList = new List<string> { "Toate" };
        categoryList.AddRange(categories.Select(c => c.Name));
        FilterPicker.ItemsSource = categoryList;
        FilterPicker.SelectedIndex = 0;
    }

    private async Task LoadExpenses()
    {
        try
        {
            _allExpenses = await _databaseService.GetExpensesAsync();
            await ApplyFilter();
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                System.Diagnostics.Debug.WriteLine($"Error loading expenses: {ex.Message}");
            });
        }
    }

    private async Task ApplyFilter()
    {
        var filtered = _allExpenses.AsEnumerable();

        if (FilterPicker.SelectedIndex > 0 && FilterPicker.SelectedItem != null)
        {
            var selectedCategory = FilterPicker.SelectedItem.ToString();
            filtered = filtered.Where(e => e.Category == selectedCategory);
        }

        var expenseViewModels = filtered
            .OrderByDescending(e => e.Date)
            .Select(e => new ExpenseViewModel
            {
                Description = e.Description,
                AmountText = $"{e.Amount:F2}",
                CategoryAndDate = $"{e.Category} • {e.Date:dd/MM/yyyy}",
                Currency = e.Currency,
                Id = e.Id
            })
            .ToList();

        ExpensesCollectionView.ItemsSource = expenseViewModels;
    }

    private async void OnClearFilterClicked(object? sender, EventArgs e)
    {
        FilterPicker.SelectedIndex = 0;
        await ApplyFilter();
    }

    private async void OnFilterChanged(object? sender, EventArgs e)
    {
        await ApplyFilter();
    }
}

public class ExpenseViewModel
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AmountText { get; set; } = string.Empty;
    public string CategoryAndDate { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}

