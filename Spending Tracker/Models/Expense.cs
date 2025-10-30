using SQLite;

namespace Spending_Tracker.Models;

[Table("expenses")]
public class Expense
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public double Amount { get; set; }
    
    public string Category { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
    
    public string Currency { get; set; } = "RON";
}

