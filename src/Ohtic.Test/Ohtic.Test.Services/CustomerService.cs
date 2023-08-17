using Mapster;
using Ohtic.Test.Data.Abstractions.Repositories;
using Ohtic.Test.Data.Collections;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Data.Models.Requests;
using Ohtic.Test.Services.Abstractions;
using Ohtic.Test.Services.Models.Dtos.Customers;

namespace Ohtic.Test.Services
{
	public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(
            ICustomerRepository repository
        )
        {
            _repository = repository;
        }

        public async Task<ReadCustomerDto> Create(CreateCustomerDto customerDto)
        {
            var request = new CustomerRequest { Customer = customerDto.Adapt<Customer>() };
            var customer = await _repository.Create(request);
            return customer.Adapt<ReadCustomerDto>();
        }

        public async Task<ReadCustomerDto> Read(int id)
        {
            var customer = await _repository.Read(id) ?? throw new KeyNotFoundException();
            return customer.Adapt<ReadCustomerDto>();
        }

        public async Task<PagedList<ReadCustomerDto>> Read(
            int pageNumber,
            int pageSize,
            string? q
        )
        {
            var customers = await _repository.Read(pageNumber, pageSize, q);
            return customers.Adapt<PagedList<ReadCustomerDto>>();
        }

        public async Task<ReadCustomerDto> Read(CustomerEmail customerEmail)
        {
            var customer = await _repository.Read(customerEmail) ?? throw new KeyNotFoundException();
            return customer.Adapt<ReadCustomerDto>();
        }

        public async Task Update(int id, UpdateCustomerDto customerDto)
        {
            var customer = await _repository.Read(id) ?? throw new KeyNotFoundException();
            var request = new CustomerRequest { Customer = customerDto.Adapt(customer) };
            await _repository.Update(request);
        }

        public async Task Delete(int id)
        {
            var customer = await _repository.Read(id) ?? throw new KeyNotFoundException();
            await _repository.Delete(customer.Id);
        }

        public async Task<bool> IsResourceOwner(int customerId, int weatherForecastId)
        {
            return await _repository.IsResourceOwner(new CustomerId(customerId), new WeatherForecastId(weatherForecastId));
        }
    }
}
