using Ohtic.Test.Data.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ohtic.Test.Services.Models.Dtos.WeatherForecasts
{
	public record struct CreateWeatherForecastDto : IWeatherForecast
    {
		[Required(AllowEmptyStrings = false)]
		[DataType(DataType.DateTime)]
		public DateTimeOffset Date { get; set; }
		[Required(AllowEmptyStrings = false)]
		[Range(-273, int.MaxValue)]
		public int TemperatureC { get; set; }
		[Required(AllowEmptyStrings = false)]
		public string Summary { get; set; }
	}
}
