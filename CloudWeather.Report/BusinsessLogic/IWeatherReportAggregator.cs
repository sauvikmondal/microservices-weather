using CloudWeather.Report.DataAccess;

namespace CloudWeather.Report.BusinsessLogic
{
    public interface IWeatherReportAggregator
    {
        public Task<WeatherReport> BuildReport(string zip, int days);
    }
}
