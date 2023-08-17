using Mapster;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Services.Models.Dtos.WeatherForecasts;

namespace Ohtic.Test.Products.Extensions
{
	internal static class TypeAdapterConfigExtensions
    {
        internal static void ConfigureMapping(
            this TypeAdapterConfig config
        )
        {
            config.ForType<UpdateWeatherForecastDto, WeatherForecast>()
				.IgnoreIf((src, dest) => !src.Date.HasValue, dest => dest.Date)
				.IgnoreIf((src, dest) => !src.TemperatureC.HasValue, dest => dest.TemperatureC)
				.IgnoreIf((src, dest) => string.IsNullOrEmpty(src.Summary), dest => dest.Summary)
                .Compile();
        }
    }
}