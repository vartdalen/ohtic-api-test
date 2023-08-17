namespace Ohtic.Test.Services.Abstractions.Nullable
{
    internal interface INullableWeatherForecast
    {
        DateTimeOffset? Date { get; }
		int? TemperatureC { get; }
		string? Summary { get; }
    }
}
