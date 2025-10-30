using SQLite;

namespace Spending_Tracker.Models;

[Table("categories")]
public class Category
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string Color { get; set; } = "#512BD4";
}

