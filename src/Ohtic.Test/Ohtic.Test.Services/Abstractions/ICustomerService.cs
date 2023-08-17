using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Services.Models.Dtos.Customers;

namespace Ohtic.Test.Services.Abstractions
{
	public interface ICustomerService :
        ICrudService<int, CreateCustomerDto, ReadCustomerDto, UpdateCustomerDto>,
        IPagedService<ReadCustomerDto>
    {
        Task<ReadCustomerDto> Read(CustomerEmail customerEmail);
        Task<bool> IsResourceOwner(int customerId, int weatherForecastId);
    }
}
