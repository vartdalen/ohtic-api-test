using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ohtic.Test.Api.Abstractions;
using Ohtic.Test.Api.Extensions;
using Ohtic.Test.Data.Models.Requests;
using Ohtic.Test.Services.Abstractions;
using Ohtic.Test.Services.Models.Dtos.WeatherForecasts;
using System.ComponentModel.DataAnnotations;

namespace Ohtic.Test.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastsController :
        ControllerBase,
        ICrudController<CreateWeatherForecastDto, UpdateWeatherForecastDto>,
        IPagedController
    {
		private readonly IWeatherForecastService _service;
		private readonly IAuthorizationService _authorizationService;

		public WeatherForecastsController(
			IWeatherForecastService weatherForecastService
		)
		{
			_service = weatherForecastService;
		}

		[Authorize(AuthenticationSchemes = "Cookie,Bearer")]
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] CreateWeatherForecastDto dto)
		{
			try
			{
				var customerId = int.Parse(HttpContext.User.FindFirst(x => x.Type == "customer_id")!.Value);
				var weatherForecast = await _service.Create(new CreateWeatherForecastRequest(dto, customerId));
				return CreatedAtAction(nameof(Get), new { id = weatherForecast.Id }, weatherForecast);
			}
			catch (Exception ex)
			{
				if (ex.IsMySqlDataTooLong()) return UnprocessableEntity();
				if (ex.IsMySqlDuplicateKeyEntry()) return Conflict();
				throw;
			}
		}

		[Authorize(AuthenticationSchemes = "Cookie,Bearer", Roles = "admin")]
		[HttpGet]
		public async Task<IActionResult> Get(
			[FromQuery][Range(1, int.MaxValue)] int? pageNumber,
			[FromQuery][Range(1, int.MaxValue)] int? pageSize,
			[FromQuery] string? q
		)
		{
			var categories = await _service.Read(pageNumber ?? 1, pageSize ?? 20, q);
			return Ok(categories);
		}

		[Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrWeatherForecastIdRouteValueMatch")]
		[HttpGet("{id:int}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var weatherForecast = await _service.Read(id);
				return Ok(weatherForecast);
			}
			catch (Exception ex)
			{
				if (ex.IsSystemNotFound()) return NotFound();
				throw;
			}
		}

		[Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrWeatherForecastIdRouteValueMatch")]
		[HttpPatch("{id:int}")]
		public async Task<IActionResult> Patch(
			int id,
			[FromBody] UpdateWeatherForecastDto dto
		)
		{
			try
			{
				await _service.Update(id, dto);
				return NoContent();
			}
			catch (Exception ex)
			{
				if (ex.IsMySqlDataTooLong()) return UnprocessableEntity();
				if (ex.IsMySqlDuplicateKeyEntry()) return Conflict();
				if (ex.IsSystemNotFound()) return NotFound();
				throw;
			}
		}

		[Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrWeatherForecastIdRouteValueMatch")]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await _service.Delete(id);
				return NoContent();
			}
			catch (Exception ex)
			{
				if (ex.IsSystemNotFound()) return NotFound();
				throw;
			}
		}
	}
}