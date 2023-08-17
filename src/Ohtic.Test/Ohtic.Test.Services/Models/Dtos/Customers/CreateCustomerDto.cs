using Ohtic.Test.Data.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace Ohtic.Test.Services.Models.Dtos.Customers
{
	public record struct CreateCustomerDto : ICustomer
    {
        [Required]
        [MaxLength(255)]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$")]
        public string Email { get; set; }
    }
}
