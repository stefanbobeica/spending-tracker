using Spending_Tracker.Models;
using Spending_Tracker.Services;
using Spending_Tracker.Pages;

namespace Spending_Tracker;

public partial class MainPage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private List<ExpenseViewModel> _allExpenses = new();
    private List<string> _categories = new();

    public MainPage()
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
        await LoadCategories();
        await LoadExpenses();
    }

    private async Task LoadCategories()
    {
        var categories = await _databaseService.GetCategoriesAsync();
        _categories = categories.Select(c => c.Name).ToList();
        _categories.Insert(0, "Toate");
        CategoryPicker.ItemsSource = _categories;
        CategoryPicker.SelectedIndex = 0;
    }

    private async Task LoadExpenses()
    {
        var expenses = await _databaseService.GetExpensesByDateRangeAsync(
            StartDatePicker.Date,
            EndDatePicker.Date.AddDays(1).AddSeconds(-1)
        );

        var categories = await _databaseService.GetCategoriesAsync();
        var categoryColors = categories.ToDictionary(c => c.Name, c => c.Color);

        _allExpenses = expenses.Select(e => new ExpenseViewModel
        {
            Expense = e,
            CategoryColor = categoryColors.ContainsKey(e.Category) 
                ? Color.FromArgb(categoryColors[e.Category]) 
                : Colors.Gray
        }).ToList();

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = _allExpenses.AsEnumerable();

        if (CategoryPicker.SelectedIndex > 0)
        {
            var selectedCategory = _categories[CategoryPicker.SelectedIndex];
            filtered = filtered.Where(e => e.Expense.Category == selectedCategory);
        }

        var filteredList = filtered.ToList();
        ExpensesListView.ItemsSource = filteredList;

        var total = filteredList.Sum(e => e.Expense.Amount);
        TotalLabel.Text = $"{total:F2} RON";
    }

    private async void OnFilterChanged(object? sender, EventArgs e)
    {
        await LoadExpenses();
    }

    private async void OnResetFilter(object? sender, EventArgs e)
    {
        var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        StartDatePicker.Date = startOfMonth;
        EndDatePicker.Date = DateTime.Now;
        CategoryPicker.SelectedIndex = 0;
        await LoadExpenses();
    }

    private async void OnAddExpense(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddEditExpensePage());
    }

    private async void OnExpenseSelected(object? sender, ItemTappedEventArgs e)
    {
        if (e.Item is ExpenseViewModel vm)
        {
            await Navigation.PushAsync(new AddEditExpensePage(vm.Expense));
        }
        ((ListView)sender!).SelectedItem = null;
    }

    private async void OnDeleteExpense(object? sender, EventArgs e)
    {
        if (sender is SwipeItem swipeItem && swipeItem.BindingContext is ExpenseViewModel vm)
        {
            bool confirm = await DisplayAlert("Confirmare", 
                $"Ștergi cheltuiala '{vm.Expense.Description}'?", 
                "Da", "Nu");
            
            if (confirm)
            {
                await _databaseService.DeleteExpenseAsync(vm.Expense);
                await LoadExpenses();
            }
        }
    }
}

public class ExpenseViewModel
{
    public Expense Expense { get; set; } = null!;
    public Color CategoryColor { get; set; } = Colors.Gray;
    
    public string AmountDisplay => $"{Expense.Amount:F2} {Expense.Currency}";
    public string DateDisplay => Expense.Date.ToString("dd MMM yyyy");
    public string Description => Expense.Description;
    public string Category => Expense.Category;
}