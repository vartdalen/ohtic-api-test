using Ohtic.Test.Data.Abstractions;
using Ohtic.Test.Data.Abstractions.Entities;

namespace Ohtic.Test.Services.Models.Dtos.WeatherForecasts
{
	public record struct ReadWeatherForecastDto : IIdentifiable, IAuditable, IWeatherForecast
    {
		public int Id { get; set; }
		public int CustomerId { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset ModifiedAt { get; set; }
		public DateTimeOffset Date { get; set; }
		public int TemperatureC { get; set; }
		public int TemperatureF { get; set; }
		public string Summary { get; set; }
	}
}
