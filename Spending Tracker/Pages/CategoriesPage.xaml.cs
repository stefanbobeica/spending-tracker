using Spending_Tracker.Models;
using Spending_Tracker.Services;

namespace Spending_Tracker.Pages;

public partial class CategoriesPage
{
    private readonly DatabaseService _databaseService;
    private Color _selectedColor = Colors.Blue;

    public CategoriesPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategories();
    }

    private async Task LoadCategories()
    {
        try
        {
            var categories = await _databaseService.GetCategoriesAsync();
            var categoryViewModels = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name,
                ColorValue = Color.FromArgb(c.Color)
            }).ToList();

            CategoriesCollectionView.ItemsSource = categoryViewModels;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading categories: {ex.Message}");
        }
    }

    private async void OnColorPickerClicked(object? sender, EventArgs e)
    {
        var colors = new[] 
        { 
            Colors.Red, Colors.Blue, Colors.Green, Colors.Orange, 
            Colors.Purple, Colors.Pink, Colors.Teal, Colors.Cyan
        };

        var result = await DisplayActionSheet("Selectează o culoare", "Anulează", null, 
            colors.Select(c => c.ToHex()).ToArray());

        if (!string.IsNullOrEmpty(result) && result != "Anulează")
        {
            _selectedColor = Color.FromArgb(result);
            ColorButton.BackgroundColor = _selectedColor;
        }
    }

    private async void OnSaveCategoryClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CategoryNameEntry.Text))
        {
            await DisplayAlert("Eroare", "Introduceți un nume pentru categorie", "OK");
            return;
        }

        var category = new Category
        {
            Name = CategoryNameEntry.Text,
            Color = _selectedColor.ToHex()
        };

        try
        {
            await _databaseService.SaveCategoryAsync(category);
            CategoryNameEntry.Text = string.Empty;
            ColorButton.BackgroundColor = Color.FromArgb("#512BD4");
            _selectedColor = Colors.Blue;
            await DisplayAlert("Succes", "Categorie adăugată", "OK");
            await LoadCategories();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Eroare", $"Nu s-a putut salva: {ex.Message}", "OK");
        }
    }

    private async void OnDeleteCategoryClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is int categoryId)
        {
            var confirm = await DisplayAlert("Confirmă", "Sigur vrei să ștergi această categorie?", "Da", "Nu");
            if (confirm)
            {
                try
                {
                    var category = new Category { Id = categoryId };
                    await _databaseService.DeleteCategoryAsync(category);
                    await DisplayAlert("Succes", "Categorie ștearsă", "OK");
                    await LoadCategories();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Eroare", $"Nu s-a putut șterge: {ex.Message}", "OK");
                }
            }
        }
    }
}

public class CategoryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Color ColorValue { get; set; }
}

