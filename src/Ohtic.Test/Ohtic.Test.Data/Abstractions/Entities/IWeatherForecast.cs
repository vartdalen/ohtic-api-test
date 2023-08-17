namespace Ohtic.Test.Data.Abstractions.Entities
{
    public interface IWeatherForecast
    {
		DateTimeOffset Date { get; }
		int TemperatureC { get; }
		string Summary { get; }
	}
}
