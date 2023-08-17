using Ohtic.Test.Services.Models.Dtos.WeatherForecasts;

namespace Ohtic.Test.Data.Models.Requests
{
	public record struct CreateWeatherForecastRequest(CreateWeatherForecastDto Dto, int CustomerId);
}
