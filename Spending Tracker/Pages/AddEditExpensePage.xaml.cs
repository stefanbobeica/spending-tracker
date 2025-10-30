using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class AddEditExpensePage
{
    private readonly DatabaseService _databaseService;
    private Expense? _expense;
    private readonly bool _isEdit;

    public AddEditExpensePage(Expense? expense = null)
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        _expense = expense;
        _isEdit = expense != null;
        
        DatePicker.Date = DateTime.Now;
        
        LoadData();
    }

    private async void LoadData()
    {
        var categories = await _databaseService.GetCategoriesAsync();
        CategoryPicker.ItemsSource = categories.Select(c => c.Name).ToList();

        if (_isEdit && _expense != null)
        {
            Title = "Editează Cheltuială";
            DescriptionEntry.Text = _expense.Description;
            AmountEntry.Text = _expense.Amount.ToString("F2");
            
            var categoryIndex = categories.FindIndex(c => c.Name == _expense.Category);
            if (categoryIndex >= 0)
                CategoryPicker.SelectedIndex = categoryIndex;
            
            DatePicker.Date = _expense.Date;
            
            var currencyIndex = CurrencyPicker.ItemsSource.Cast<string>().ToList().IndexOf(_expense.Currency);
            if (currencyIndex >= 0)
                CurrencyPicker.SelectedIndex = currencyIndex;
        }
        else
        {
            Title = "Adaugă Cheltuială";
            if (categories.Count > 0)
                CategoryPicker.SelectedIndex = 0;
        }
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(DescriptionEntry.Text))
        {
            await DisplayAlert("Eroare", "Introdu o descriere", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(AmountEntry.Text) || !double.TryParse(AmountEntry.Text, out double amount))
        {
            await DisplayAlert("Eroare", "Introdu o sumă validă", "OK");
            return;
        }

        if (CategoryPicker.SelectedIndex < 0)
        {
            await DisplayAlert("Eroare", "Selectează o categorie", "OK");
            return;
        }

        if (_expense == null)
        {
            _expense = new Expense();
        }

        _expense.Description = DescriptionEntry.Text;
        _expense.Amount = amount;
        _expense.Category = CategoryPicker.SelectedItem.ToString()!;
        _expense.Date = DatePicker.Date;
        _expense.Currency = CurrencyPicker.SelectedItem?.ToString() ?? "RON";

        await _databaseService.SaveExpenseAsync(_expense);
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

