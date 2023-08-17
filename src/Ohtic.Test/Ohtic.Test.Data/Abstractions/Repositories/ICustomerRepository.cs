using Ohtic.Test.Abstractions.Repositories;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Data.Models.Requests;

namespace Ohtic.Test.Data.Abstractions.Repositories
{
	public interface ICustomerRepository :
        ICrudRepository<int, Customer, CustomerRequest>,
        IPagedQueryRepository<Customer>
    {
        Task<Customer?> Read(CustomerEmail customerEmail);
        Task<bool> IsResourceOwner(CustomerId customerId, WeatherForecastId orderId);
    }
}
