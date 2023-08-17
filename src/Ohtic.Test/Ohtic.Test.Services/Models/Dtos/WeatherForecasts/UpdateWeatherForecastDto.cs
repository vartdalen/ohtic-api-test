using Ohtic.Test.Services.Abstractions.Nullable;

namespace Ohtic.Test.Services.Models.Dtos.WeatherForecasts
{
	public record struct UpdateWeatherForecastDto : INullableWeatherForecast
	{
		public DateTimeOffset? Date { get; set; }
		public int? TemperatureC { get; set; }
		public string? Summary { get; set; }
	}
}
