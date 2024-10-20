// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using CloudWeather.Dataloader.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables() 
    .Build();


var servicesConfig = config.GetSection("Services");

var tempServiceConfig = servicesConfig.GetSection("Temparature");
var tempServiceHost = tempServiceConfig["Host"];
var tempServicePort = tempServiceConfig["Port"];

var precipServiceConfig = servicesConfig.GetSection("Precipitation");
var precipServiceHost = precipServiceConfig["Host"];
var precipServicePort = precipServiceConfig["Port"];

var zipCodes= new List<string>
{
    "73026",
    "68104",
    "04401",
    "32808",
    "19717"
};


Console.WriteLine("Starting Data Load");

var temparatureHttpClient= new HttpClient();
temparatureHttpClient.BaseAddress = new Uri($"http://{tempServiceHost}:{tempServicePort}");

var precipitationHttpClient=new HttpClient();
precipitationHttpClient.BaseAddress =new Uri($"http://{precipServiceHost}:{precipServicePort}");

foreach (var zip in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {zip}");
    var from= DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;

    for (var day = from.Date; day.Date<=thru.Date; day=day.AddDays(1)) {
        var temps = PostTemp(zip, day, temparatureHttpClient);
        PostPrecip(temps[0], zip, day, precipitationHttpClient);
    }
}

void PostPrecip(int lowTemp, string zip, DateTime day, HttpClient precipitationHttpClient)
{
    var rand= new Random();
    var isPrecip = rand.Next(2) < 1;
    PrecipitationModel precipitation;

    if (isPrecip)
    {
        var precipInches=rand.Next(1,16);
        if (lowTemp < 32)
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "snow",
                ZipCode = zip,
                CreatedOn = day
            };
        }
        else
        {
            precipitation = new PrecipitationModel
            {
                AmountInches = precipInches,
                WeatherType = "rain",
                ZipCode = zip,
                CreatedOn = day
            };

        }
    }
    else
    {
        precipitation = new PrecipitationModel
        {
            AmountInches = 0,
            WeatherType = "none",
            ZipCode = zip,
            CreatedOn = day
        };
    }

    var precipResponse = precipitationHttpClient
        .PostAsJsonAsync("observation", precipitation)
        .Result;

    if(precipResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Precipitation: Date: {day:d}"+
                          $"Zip: {zip}"+
                          $"Type: {precipitation.WeatherType}"+
                          $"Amount (in.): {precipitation.AmountInches}");
    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient temparatureHttpClient)
{
    var rand = new Random();
    var t1 = rand.Next(0, 100);
    var t2 = rand.Next(0, 100);
    var hiloTemps = new List<int>{t1, t2};
    hiloTemps.Sort();

    var temparatureObservation = new TemparatureModel
    {
        TempLowF = hiloTemps[0],
        TempHighF = hiloTemps[1],
        ZipCode = zip,
        CreatedOn = day
    };

    var temparatureResponse = temparatureHttpClient
        .PostAsJsonAsync("observation",temparatureObservation) 
        .Result;
    
    if (temparatureResponse.IsSuccessStatusCode)
    {
        Console.WriteLine($"Posted Temparature: Date: {day:d}" +
                          $"Zip: {zip}" +
                          $"Low (F): {temparatureObservation.TempLowF}" +
                          $"High (F): {temparatureObservation.TempHighF}");
    }
    else
    {
        Console.WriteLine(temparatureResponse.ToString());
    }

    return hiloTemps;
}