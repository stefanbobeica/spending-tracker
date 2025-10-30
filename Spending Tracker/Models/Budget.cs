using SQLite;

namespace Spending_Tracker.Models;

[Table("budgets")]
public class Budget
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public double MonthlyLimit { get; set; }
    
    public int Month { get; set; }
    
    public int Year { get; set; }
}

