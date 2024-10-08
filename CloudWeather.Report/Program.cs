using CloudWeather.Report.BusinsessLogic;
using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggregator, WeatherReportAggregator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig"));

// Add services to the container.
builder.Services.AddDbContext<WeatherReportDbContext>(
    opts =>
    {
        opts.EnableSensitiveDataLogging();
        opts.EnableDetailedErrors();
        opts.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
    }, ServiceLifetime.Transient);


var app = builder.Build();

app.MapGet("/weather-report/{zip}",
    async (string zip, [FromQuery] int? days, IWeatherReportAggregator weatherAgg) => {
        if(days == null || days < 1 || days > 30){
            return Results.BadRequest("Please provide a days parameter between 1 and 30");
        }
        var report = await weatherAgg.BuildReport(zip, days.Value);
        return Results.Ok(report);
});

app.Run();
