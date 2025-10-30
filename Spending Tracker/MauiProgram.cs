﻿using Microsoft.Extensions.Logging;
using Spending_Tracker.Services;
using Spending_Tracker.Pages;

namespace Spending_Tracker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register services
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<CurrencyService>();
        builder.Services.AddSingleton<PdfReportService>();

        // Register pages
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<AddEditExpensePage>();
        builder.Services.AddTransient<StatisticsPage>();
        builder.Services.AddTransient<CurrencyConverterPage>();
        builder.Services.AddTransient<SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}