using CloudWeather.Report.Config;
using CloudWeather.Report.DataAccess;
using CloudWeather.Report.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace CloudWeather.Report.BusinsessLogic
{
    public class WeatherReportAggregator : IWeatherReportAggregator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<WeatherReportAggregator> _logger;
        private readonly WeatherDataConfig _weatherDataConfig;
        private readonly WeatherReportDbContext _db;
        public WeatherReportAggregator(
            IHttpClientFactory http, 
            ILogger<WeatherReportAggregator> logger, 
            IOptions<WeatherDataConfig> weatherDataConfig, 
            WeatherReportDbContext db)
        {
            _http = http;
            _logger = logger;
            _weatherDataConfig = weatherDataConfig.Value;
            _db = db;
        }
        public async Task<WeatherReport> BuildReport(string zip, int days)
        {
            var httpClient = _http.CreateClient();
            var precipData = await FetchPrecipitationData(httpClient, zip, days);
            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);
            var tempData = await FetchTemparatureData(httpClient, zip, days);
            var averageHighTemp = tempData.Average(t => t.TempHighF);
            var averageLowTemp = tempData.Average(t => t.TempLowF);

            _logger.LogInformation(
                $"zip: {zip} over last {days} days: " +
                $"lo temp: {averageLowTemp}, hi temp: {averageHighTemp}"
                );

            var weatherReport = new WeatherReport
            {
                AverageHighF = Math.Round(averageHighTemp, 1),
                AverageLowF = Math.Round(averageLowTemp, 1),
                RainTotalInches = totalRain,
                SnowTotalInches= totalSnow,
                ZipCode = zip,
                CreatedOn = DateTime.UtcNow,
            };

            _db.Add(weatherReport);
            await _db.SaveChangesAsync();
            return weatherReport;
        }

        private static decimal GetTotalSnow(IEnumerable<PrecipitationModel> precipData)
        {
            var totalSnow = precipData
                .Where(p => p.WeatherTypes == "snow")
                .Sum(p => p.AmountInches);
            return Math.Round(totalSnow,1);
        }

        private static decimal GetTotalRain(IEnumerable<PrecipitationModel> precipData)
        {
            var totalRain = precipData
               .Where(p => p.WeatherTypes == "rain")
               .Sum(p => p.AmountInches);
            return Math.Round(totalRain, 1);
        }

        private async Task<List<TemparatureModel>> FetchTemparatureData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildTemparatureServiceEndpoint(zip, days);
            var temparatureRecords = await httpClient.GetAsync(endpoint);
            var jsonSerlializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var temparatureData = await temparatureRecords
                .Content
                .ReadFromJsonAsync<List<TemparatureModel>>(jsonSerlializerOptions);
            return temparatureData ?? new List<TemparatureModel>();
        }

        private string? BuildTemparatureServiceEndpoint(string zip, int days)
        {
            var tempServicePrtocol = _weatherDataConfig.TempDataProtocol;
            var tempServiceHost= _weatherDataConfig.TempDataHost;
            var tempServicePort = _weatherDataConfig.TempDataPort;
            return $"{tempServicePrtocol}://{tempServiceHost}:{tempServicePort}/observation/{zip}?days={days}";
        }

        private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildPrecipitationServiceEndpoint(zip, days);
            var precipRecords = await httpClient.GetAsync(endpoint);
            var jsonSerlializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var precipData = await precipRecords
                .Content
                .ReadFromJsonAsync<List<PrecipitationModel>>(jsonSerlializerOptions);
            return precipData ?? new List<PrecipitationModel>();
        }

        private string? BuildPrecipitationServiceEndpoint(string zip, int days)
        {
            var precipServicePrtocol = _weatherDataConfig.PrecipDataProtocol;
            var precipServiceHost = _weatherDataConfig.PrecipDataHost;
            var precipServicePort = _weatherDataConfig.PrecipDataPort;
            return $"{precipServicePrtocol}://{precipServiceHost}:{precipServicePort}/observation/{zip}?days={days}";
        }
    }
}
