using Ohtic.Test.Abstractions.Repositories;
using Ohtic.Test.Data.Collections;
using Ohtic.Test.Data.Extensions;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Requests;

namespace Ohtic.Test.Data.Repositories
{
	public class WeatherForecastRepository : IWeatherForecastRepository
    {
        private readonly WeatherForecastsDbContext _context;

        public WeatherForecastRepository(WeatherForecastsDbContext context)
        {
            _context = context;
        }

        public async Task<WeatherForecast> Create(WeatherForecastRequest request)
        {
            var result = await _context.WeatherForecasts.AddAsync(request.WeatherForecast);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<WeatherForecast?> Read(int id)
        {
            return await _context.WeatherForecasts.FindAsync(id);
        }

        public async Task<PagedList<WeatherForecast>> Read(
            int pageNumber,
            int pageSize,
            string? q
        )
        {
            var categories = _context.WeatherForecasts.Conditional(
                !string.IsNullOrEmpty(q),
                x => x.Where(f => f.Summary.Contains(q!))
            );
            return await PagedList<WeatherForecast>.Create(categories, pageNumber, pageSize);
        }

        public async Task Update(WeatherForecastRequest request)
        {
            _context.WeatherForecasts.Update(request.WeatherForecast);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var category = await Read(id);
            if (category is not null)
            {
                _context.WeatherForecasts.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
