using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Requests;

namespace Ohtic.Test.Abstractions.Repositories
{
	public interface IWeatherForecastRepository :
        ICrudRepository<int, WeatherForecast, WeatherForecastRequest>,
        IPagedQueryRepository<WeatherForecast>
    {
    }
}
