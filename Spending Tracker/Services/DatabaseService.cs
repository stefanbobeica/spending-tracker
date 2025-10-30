using SQLite;
using Spending_Tracker.Models;

namespace Spending_Tracker.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _database;

    public async Task InitAsync()
    {
        if (_database != null)
            return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "expenses.db");
        _database = new SQLiteAsyncConnection(dbPath);
        
        await _database.CreateTableAsync<Expense>();
        await _database.CreateTableAsync<Category>();
        await _database.CreateTableAsync<Budget>();
        
        await SeedDefaultCategories();
    }

    private async Task SeedDefaultCategories()
    {
        var count = await _database!.Table<Category>().CountAsync();
        if (count == 0)
        {
            await _database.InsertAllAsync(new List<Category>
            {
                new() { Name = "Alimente", Color = "#FF6B6B" },
                new() { Name = "Transport", Color = "#4ECDC4" },
                new() { Name = "Divertisment", Color = "#45B7D1" },
                new() { Name = "Utilități", Color = "#FFA07A" },
                new() { Name = "Sănătate", Color = "#98D8C8" },
                new() { Name = "Educație", Color = "#F7DC6F" },
                new() { Name = "Altele", Color = "#BB8FCE" }
            });
        }
    }

    // Expense operations
    public async Task<List<Expense>> GetExpensesAsync()
    {
        await InitAsync();
        return await _database!.Table<Expense>().OrderByDescending(e => e.Date).ToListAsync();
    }

    public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime start, DateTime end)
    {
        await InitAsync();
        return await _database!.Table<Expense>()
            .Where(e => e.Date >= start && e.Date <= end)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<int> SaveExpenseAsync(Expense expense)
    {
        await InitAsync();
        if (expense.Id != 0)
            return await _database!.UpdateAsync(expense);
        return await _database!.InsertAsync(expense);
    }

    public async Task<int> DeleteExpenseAsync(Expense expense)
    {
        await InitAsync();
        return await _database!.DeleteAsync(expense);
    }

    // Category operations
    public async Task<List<Category>> GetCategoriesAsync()
    {
        await InitAsync();
        return await _database!.Table<Category>().ToListAsync();
    }

    public async Task<int> SaveCategoryAsync(Category category)
    {
        await InitAsync();
        if (category.Id != 0)
            return await _database!.UpdateAsync(category);
        return await _database!.InsertAsync(category);
    }

    public async Task<int> DeleteCategoryAsync(Category category)
    {
        await InitAsync();
        return await _database!.DeleteAsync(category);
    }

    // Budget operations
    public async Task<List<Budget>> GetBudgetsAsync()
    {
        await InitAsync();
        return await _database!.Table<Budget>().ToListAsync();
    }

    public async Task<List<Budget>> GetBudgetsForMonthAsync(int month, int year)
    {
        await InitAsync();
        return await _database!.Table<Budget>()
            .Where(b => b.Month == month && b.Year == year)
            .ToListAsync();
    }

    public async Task<int> SaveBudgetAsync(Budget budget)
    {
        await InitAsync();
        if (budget.Id != 0)
            return await _database!.UpdateAsync(budget);
        return await _database!.InsertAsync(budget);
    }

    public async Task<int> DeleteBudgetAsync(Budget budget)
    {
        await InitAsync();
        return await _database!.DeleteAsync(budget);
    }
}

