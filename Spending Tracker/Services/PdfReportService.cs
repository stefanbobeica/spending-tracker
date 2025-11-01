using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Spending_Tracker.Models;
using PdfColors = QuestPDF.Helpers.Colors;

namespace Spending_Tracker.Services;

public class PdfReportService
{
    private readonly DatabaseService _databaseService;
    private readonly CurrencyService _currencyService;

    public PdfReportService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        _currencyService = new CurrencyService();
        QuestPDF.Settings.License = LicenseType.Community;
    }
    
    private async Task<double> ConvertToRonAsync(double amount, string currency)
    {
        if (currency == "RON")
            return amount;
            
        // For currencies other than RON, we need to convert back to RON
        // The API gives rates from RON to other currencies (RON -> Currency)
        // So to convert back: amount / rate
        var rates = await _currencyService.GetExchangeRatesAsync();
        
        if (rates.ContainsKey(currency) && rates[currency] > 0)
        {
            return amount / rates[currency];
        }
        
        return amount; // If rate not found, return as-is
    }

    public async Task<string> GenerateMonthlyReportAsync(DateTime startDate, DateTime endDate)
    {
        var expenses = await _databaseService.GetExpensesByDateRangeAsync(startDate, endDate);
        
        // Convert all amounts to RON
        var expensesInRon = new List<(Expense expense, double amountInRon)>();
        foreach (var expense in expenses)
        {
            var amountInRon = await ConvertToRonAsync(expense.Amount, expense.Currency);
            expensesInRon.Add((expense, amountInRon));
        }
        
        // Calculate totals in RON for the report
        var totalInRon = expensesInRon.Sum(e => e.amountInRon);
        var expenseCount = expenses.Count;
        var days = Math.Max((endDate - startDate).Days, 1);
        var dailyAverage = expenseCount > 0 ? totalInRon / days : 0;
        
        var fileName = $"Raport_Cheltuieli_{startDate:yyyy_MM_dd}_to_{endDate:yyyy_MM_dd}.pdf";
        var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(PdfColors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Height(100)
                    .Background(PdfColors.Blue.Medium)
                    .Padding(20)
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text("💰 SPENDING TRACKER")
                            .FontSize(24)
                            .Bold()
                            .FontColor(PdfColors.White);
                        column.Item().PaddingTop(5).Text("Raport Cheltuieli Lunare")
                            .FontSize(14)
                            .FontColor(PdfColors.White);
                        column.Item().PaddingTop(5).Text($"Perioada: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}")
                            .FontSize(10)
                            .FontColor(PdfColors.Grey.Lighten3);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        // Summary Section
                        column.Item().PaddingBottom(15).Row(row =>
                        {
                            row.RelativeItem().AlignLeft().Column(col =>
                            {
                                col.Item().Background(PdfColors.Blue.Lighten4).Padding(15).Column(summaryCol =>
                                {
                                    summaryCol.Item().Text("📊 SUMAR").FontSize(14).Bold().FontColor(PdfColors.Blue.Darken2);
                                    summaryCol.Item().PaddingTop(8).Text($"Total Cheltuieli: {totalInRon:F2} RON")
                                        .FontSize(12).SemiBold();
                                    summaryCol.Item().Text($"Număr Tranzacții: {expenseCount}")
                                        .FontSize(11);
                                    summaryCol.Item().Text($"Media pe Zi: {dailyAverage:F2} RON")
                                        .FontSize(11);
                                });
                            });
                        });

                        // Category Breakdown
                        column.Item().PaddingBottom(15).Column(catColumn =>
                        {
                            catColumn.Item().PaddingBottom(10).Text("📁 CHELTUIELI PE CATEGORII")
                                .FontSize(14).Bold().FontColor(PdfColors.Blue.Darken2);

                            var categoryGroups = expensesInRon
                                .GroupBy(e => e.expense.Category)
                                .Select(g => new
                                {
                                    Category = g.Key,
                                    Total = g.Sum(e => e.amountInRon),
                                    Count = g.Count(),
                                    Percentage = totalInRon > 0
                                        ? (g.Sum(e => e.amountInRon) / totalInRon * 100)
                                        : 0
                                })
                                .OrderByDescending(x => x.Total);

                            foreach (var group in categoryGroups)
                            {
                                catColumn.Item().PaddingBottom(8).Background(PdfColors.Grey.Lighten4).Padding(12).Row(row =>
                                {
                                    row.RelativeItem().Column(col =>
                                    {
                                        col.Item().Text(group.Category ?? "N/A").FontSize(12).SemiBold();
                                        col.Item().Text($"{group.Count} tranzacții").FontSize(9).FontColor(PdfColors.Grey.Medium);
                                    });
                                    row.ConstantItem(150).AlignRight().Column(col =>
                                    {
                                        col.Item().Text($"{group.Total:F2} RON").FontSize(12).Bold().FontColor(PdfColors.Blue.Medium);
                                        col.Item().Text($"{group.Percentage:F1}%").FontSize(9).FontColor(PdfColors.Grey.Medium);
                                    });
                                });
                            }
                        });

                        // Detailed Transactions - with pagination support
                        column.Item().PaddingTop(15).Column(detailColumn =>
                        {
                            detailColumn.Item().PaddingBottom(10).Text("📋 DETALII TRANZACȚII")
                                .FontSize(14).Bold().FontColor(PdfColors.Blue.Darken2);

                            // Table Header
                            detailColumn.Item().Background(PdfColors.Blue.Medium).Padding(8).Row(row =>
                            {
                                row.RelativeItem().Text("Data").FontSize(10).Bold().FontColor(PdfColors.White);
                                row.RelativeItem().Text("Categorie").FontSize(10).Bold().FontColor(PdfColors.White);
                                row.RelativeItem(2).Text("Descriere").FontSize(10).Bold().FontColor(PdfColors.White);
                                row.ConstantItem(80).AlignRight().Text("Sumă (RON)").FontSize(10).Bold().FontColor(PdfColors.White);
                            });

                            // Table Rows - limit to prevent space issues
                            var sortedExpensesList = expensesInRon.OrderByDescending(e => e.expense.Date).Take(100).ToList();
                            
                            // Use dynamic container for each row to allow page breaks
                            for (int index = 0; index < sortedExpensesList.Count; index++)
                            {
                                var expenseData = sortedExpensesList[index];
                                var expense = expenseData.expense;
                                var amountInRon = expenseData.amountInRon;
                                
                                var bgColor = index % 2 == 0 
                                    ? PdfColors.White 
                                    : PdfColors.Grey.Lighten5;

                                detailColumn.Item()
                                    .Background(bgColor)
                                    .Padding(6)
                                    .BorderBottom(0.5f)
                                    .BorderColor(PdfColors.Grey.Lighten2)
                                    .Row(row =>
                                    {
                                        row.RelativeItem().Text(expense.Date.ToString("dd/MM/yyyy")).FontSize(9);
                                        row.RelativeItem().Text(expense.Category ?? "N/A").FontSize(9);
                                        row.RelativeItem(2).Text(expense.Description ?? "N/A").FontSize(9);
                                        row.ConstantItem(80).AlignRight().Text(amountInRon.ToString("F2")).FontSize(9).SemiBold();
                                    });
                            }
                            
                            // Show message if there are more transactions
                            if (expenses.Count > 100)
                            {
                                detailColumn.Item().PaddingTop(10).Text($"Afișate primele 100 din {expenses.Count} tranzacții")
                                    .FontSize(10).Italic().FontColor(PdfColors.Grey.Medium);
                            }
                        });
                    });

                page.Footer()
                    .Height(30)
                    .Background(PdfColors.Grey.Lighten3)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generat pe ").FontSize(9);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).SemiBold();
                        text.Span(" | Pagina ").FontSize(9);
                        text.CurrentPageNumber().FontSize(9);
                        text.Span(" din ").FontSize(9);
                        text.TotalPages().FontSize(9);
                    });
            });
        });

        document.GeneratePdf(filePath);
        
        return filePath;
    }

    public async Task<string> GenerateCategoryReportAsync(string categoryName, DateTime startDate, DateTime endDate)
    {
        var allExpenses = await _databaseService.GetExpensesByDateRangeAsync(startDate, endDate);
        var expenses = allExpenses.Where(e => e.Category == categoryName).ToList();
        
        // Convert all amounts to RON
        var expensesInRon = new List<(Expense expense, double amountInRon)>();
        foreach (var expense in expenses)
        {
            var amountInRon = await ConvertToRonAsync(expense.Amount, expense.Currency);
            expensesInRon.Add((expense, amountInRon));
        }
        
        var fileName = $"Raport_{categoryName}_{startDate:yyyy_MM_dd}_to_{endDate:yyyy_MM_dd}.pdf";
        var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(PdfColors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Height(100)
                    .Background(PdfColors.Green.Medium)
                    .Padding(20)
                    .AlignCenter()
                    .Column(column =>
                    {
                        column.Item().Text($"📁 RAPORT CATEGORIE: {categoryName.ToUpper()}")
                            .FontSize(20)
                            .Bold()
                            .FontColor(PdfColors.White);
                        column.Item().PaddingTop(5).Text($"Perioada: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}")
                            .FontSize(12)
                            .FontColor(PdfColors.White);
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        // Summary using converted RON amounts
                        var totalCategoryInRon = expensesInRon.Sum(e => e.amountInRon);
                        var avgInRon = expensesInRon.Count > 0 ? expensesInRon.Average(e => e.amountInRon) : 0;
                        var maxInRon = expensesInRon.Count > 0 ? expensesInRon.Max(e => e.amountInRon) : 0;
                        var minInRon = expensesInRon.Count > 0 ? expensesInRon.Min(e => e.amountInRon) : 0;
                        
                        column.Item().PaddingBottom(20).Background(PdfColors.Green.Lighten4).Padding(20).Column(summaryCol =>
                        {
                            summaryCol.Item().Text("📊 SUMAR").FontSize(14).Bold().FontColor(PdfColors.Green.Darken2);
                            summaryCol.Item().PaddingTop(8).Text($"Total Cheltuieli: {totalCategoryInRon:F2} RON")
                                .FontSize(14).Bold();
                            summaryCol.Item().Text($"Număr Tranzacții: {expenses.Count}")
                                .FontSize(12);
                            summaryCol.Item().Text($"Media pe Tranzacție: {avgInRon:F2} RON")
                                .FontSize(12);
                            summaryCol.Item().Text($"Cheltuială Maximă: {maxInRon:F2} RON")
                                .FontSize(12);
                            summaryCol.Item().Text($"Cheltuială Minimă: {minInRon:F2} RON")
                                .FontSize(12);
                        });

                        // Transactions - with pagination support
                        column.Item().Column(detailColumn =>
                        {
                            detailColumn.Item().PaddingBottom(10).Text("📋 TOATE TRANZACȚIILE")
                                .FontSize(14).Bold().FontColor(PdfColors.Green.Darken2);

                            var sortedTransactions = expenses.OrderByDescending(e => e.Date).Take(50).ToList();
                            
                            foreach (var expense in sortedTransactions)
                            {
                                detailColumn.Item()
                                    .PaddingBottom(10)
                                    .Background(PdfColors.Grey.Lighten5)
                                    .Padding(15)
                                    .Column(expCol =>
                                    {
                                        expCol.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text(expense.Description ?? "N/A").FontSize(13).Bold();
                                            row.ConstantItem(120).AlignRight().Text($"{expense.Amount:F2} RON")
                                                .FontSize(13).Bold().FontColor(PdfColors.Green.Medium);
                                        });
                                        expCol.Item().PaddingTop(5).Text($"📅 {expense.Date:dd/MM/yyyy}")
                                            .FontSize(10).FontColor(PdfColors.Grey.Medium);
                                    });
                            }
                            
                            // Show message if there are more transactions
                            if (expenses.Count > 50)
                            {
                                detailColumn.Item().PaddingTop(10).Text($"Afișate primele 50 din {expenses.Count} tranzacții")
                                    .FontSize(10).Italic().FontColor(PdfColors.Grey.Medium);
                            }
                        });
                    });

                page.Footer()
                    .Height(30)
                    .Background(PdfColors.Grey.Lighten3)
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generat pe ").FontSize(9);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).SemiBold();
                    });
            });
        });

        document.GeneratePdf(filePath);
        
        return filePath;
    }
}

