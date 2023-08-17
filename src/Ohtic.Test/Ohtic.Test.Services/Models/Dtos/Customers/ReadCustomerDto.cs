using Ohtic.Test.Data.Abstractions;
using Ohtic.Test.Data.Abstractions.Entities;

namespace Ohtic.Test.Services.Models.Dtos.Customers
{
	public record struct ReadCustomerDto : IIdentifiable, IAuditable, ICustomer
	{
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public string Email { get; set; }
    }
}
