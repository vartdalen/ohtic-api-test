using Ohtic.Test.Data.Models.Entities;

namespace Ohtic.Test.Data.Models.Requests
{
    public record struct WeatherForecastRequest(WeatherForecast WeatherForecast);
	public record struct CustomerRequest(Customer Customer);

}
