using Mapster;
using Ohtic.Test.Abstractions.Repositories;
using Ohtic.Test.Data.Collections;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Requests;
using Ohtic.Test.Services.Abstractions;
using Ohtic.Test.Services.Models.Dtos.WeatherForecasts;

namespace Ohtic.Test.Services
{
	public class WeatherForecastService : IWeatherForecastService
    {
        private readonly IWeatherForecastRepository _repository;

        public WeatherForecastService(
            IWeatherForecastRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<ReadWeatherForecastDto> Create(CreateWeatherForecastRequest weatherForecastRequest)
        {
            var request = new WeatherForecastRequest { WeatherForecast = weatherForecastRequest.Dto.Adapt<WeatherForecast>() };
            request.WeatherForecast.CustomerId = weatherForecastRequest.CustomerId;
            var weatherForecast = await _repository.Create(request);
            return weatherForecast.Adapt<ReadWeatherForecastDto>();
        }

        public async Task<ReadWeatherForecastDto> Read(int id)
        {
            var weatherForecast = await _repository.Read(id) ?? throw new KeyNotFoundException();
            return weatherForecast.Adapt<ReadWeatherForecastDto>();
        }

        public async Task<PagedList<ReadWeatherForecastDto>> Read(
            int pageNumber,
            int pageSize,
            string? q
        )
        {
            var categories = await _repository.Read(pageNumber, pageSize, q);
            return categories.Adapt<PagedList<ReadWeatherForecastDto>>();
        }

        public async Task Update(int id, UpdateWeatherForecastDto weatherForecastDto)
        {
            var weatherForecast = await _repository.Read(id) ?? throw new KeyNotFoundException();
            var request = new WeatherForecastRequest { WeatherForecast = weatherForecastDto.Adapt(weatherForecast) };
            await _repository.Update(request);
        }

        public async Task Delete(int id)
        {
            var weatherForecast = await _repository.Read(id) ?? throw new KeyNotFoundException();
            await _repository.Delete(weatherForecast.Id);
        }
    }
}
