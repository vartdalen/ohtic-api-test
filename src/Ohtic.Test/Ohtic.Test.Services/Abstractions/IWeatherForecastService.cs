using Ohtic.Test.Data.Models.Requests;
using Ohtic.Test.Services.Models.Dtos.WeatherForecasts;

namespace Ohtic.Test.Services.Abstractions
{
	public interface IWeatherForecastService :
        ICrudService<int, CreateWeatherForecastRequest, ReadWeatherForecastDto, UpdateWeatherForecastDto>,
        IPagedService<ReadWeatherForecastDto>
    {
    }
}
