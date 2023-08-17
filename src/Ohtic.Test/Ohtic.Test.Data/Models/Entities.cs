using Ohtic.Test.Data.Abstractions;
using Ohtic.Test.Data.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ohtic.Test.Data.Models.Entities
{
	[Table("weather_forecasts")]
	public class WeatherForecast : IIdentifiable, IAuditable, IWeatherForecast
	{
		[Column("id")]
		public int Id { get; set; }
		[Column("created_at")]
		public DateTimeOffset CreatedAt { get; set; }
		[Column("modified_at")]
		public DateTimeOffset ModifiedAt { get; set; }
		[Column("date")]
		public DateTimeOffset Date { get; set; }
		[Column("temperature_c")]
		public int TemperatureC { get; set; }
		[Column("temperature_f")]
		public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		[Column("sumary")]
		public string Summary { get; set; }

		[Column("customer_id")]
		public int CustomerId { get; set; }
		public virtual Customer Customer { get; set; }
	}

	[Table("customers")]
	public class Customer : ICustomer, IIdentifiable, IAuditable
	{
		[Column("id")]
		public int Id { get; set; }
		[Column("created_at")]
		public DateTimeOffset CreatedAt { get; set; }
		[Column("modified_at")]
		public DateTimeOffset ModifiedAt { get; set; }
		[Column("email")]
		public string Email { get; set; }

		public virtual ICollection<WeatherForecast> WeatherForecasts { get; set; }
	}
}