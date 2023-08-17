using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ohtic.Test.Api.Abstractions;
using Ohtic.Test.Api.Extensions;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Services.Abstractions;
using Ohtic.Test.Services.Models.Dtos.Customers;
using System.ComponentModel.DataAnnotations;

namespace Ohtic.Test.Api.Controllers
{
	[ApiController]
    [Route("[controller]")]
    public class CustomersController :
        ControllerBase,
        ICrudController<CreateCustomerDto, UpdateCustomerDto>,
        IPagedController
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerDto dto)
        {
            try
            {
                var customer = await _customerService.Create(dto);
                return CreatedAtAction(nameof(Get), new { id = customer.Id }, customer);
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
            var customers = await _customerService.Read(pageNumber ?? 1, pageSize ?? 20, q);
            return Ok(customers);
        }

        [Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrCustomerIdRouteValueMatch")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var customer = await _customerService.Read(id);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                if (ex.IsSystemNotFound()) return NotFound().WithInvalidIdentifier(id.ToString());
                throw;
            }
        }

        [Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrCustomerEmailRouteValueMatch")]
        [HttpGet("{email:regex(^[[a-zA-Z0-9.!#$%&'*+\\/=?^_`{{|}}~-]]+@[[a-zA-Z0-9]](?:[[a-zA-Z0-9-]]{{0,61}}[[a-zA-Z0-9]])?(?:\\.[[a-zA-Z0-9]](?:[[a-zA-Z0-9-]]{{0,61}}[[a-zA-Z0-9]])?)*$)}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var customer = await _customerService.Read(new CustomerEmail(email));
                return Ok(customer);
            }
            catch (Exception ex)
            {
                if (ex.IsSystemNotFound()) return NotFound().WithInvalidIdentifier(email);
                throw;
            }
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "admin")]
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> Patch(
            int id,
            [FromBody] UpdateCustomerDto dto
        )
        {
            try
            {
                await _customerService.Update(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.IsMySqlDataTooLong()) return UnprocessableEntity();
                if (ex.IsMySqlDuplicateKeyEntry()) return Conflict();
                if (ex.IsSystemNotFound()) return NotFound().WithInvalidIdentifier(id.ToString());
                throw;
            }
        }

        [Authorize(AuthenticationSchemes = "Cookie,Bearer", Policy = "AdminOrCustomerHashIdRouteValueMatch")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                if (ex.IsSystemNotFound()) return NotFound().WithInvalidIdentifier(id.ToString());
                throw;
            }
        }
    }
}