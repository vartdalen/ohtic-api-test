using MockQueryable.NSubstitute;
using Ohtic.Test.Data.Abstractions.Repositories;
using Ohtic.Test.Data.Collections;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Data.Models.Requests;
using Ohtic.Test.Services;
using Ohtic.Test.Services.Models.Dtos.Customers;
using System.Collections.ObjectModel;

namespace Ohtic.Test.Tests
{
	public class CustomerServiceTests
	{
		[Fact]
		public async Task Create_CustomerCreatedSuccessfully_ReturnsCreatedCustomer()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var createDto = new CreateCustomerDto { Email = "customer@example.com" };
			var expectedCustomer = new Customer
			{
				Id = 1,
				CreatedAt = DateTimeOffset.UtcNow,
				ModifiedAt = DateTimeOffset.UtcNow,
				Email = createDto.Email,
				WeatherForecasts = new Collection<WeatherForecast>()
			};

			repositoryMock.Create(Arg.Any<CustomerRequest>()).Returns(expectedCustomer);

			// Act
			var result = await customerService.Create(createDto);

			// Assert
			Assert.Equal(1, result.Id);
			Assert.Equal(createDto.Email, result.Email);
		}

		[Fact]
		public async Task Read_ValidId_ReturnsCustomer()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var customerId = 1;
			var expectedCustomer = new Customer { Email = "customer@example.com" };

			repositoryMock.Read(customerId).Returns(expectedCustomer);

			// Act
			var result = await customerService.Read(customerId);

			// Assert
			Assert.Equal(expectedCustomer.Email, result.Email);
			// Add more specific assertions based on your scenario
		}

		[Fact]
		public async Task Read_PageNumberAndPageSize_ReturnsPagedCustomers()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var pageNumber = 1;
			var pageSize = 3;
			var query = "customer";
			var customers = new Collection<Customer>
			{
				new Customer
				{
					Id = 1,
					CreatedAt = DateTimeOffset.UtcNow,
					ModifiedAt = DateTimeOffset.UtcNow,
					Email = "customer1@example.com",
					WeatherForecasts = new Collection<WeatherForecast>()
				},
				new Customer
				{
					Id = 2,
					CreatedAt = DateTimeOffset.UtcNow,
					ModifiedAt = DateTimeOffset.UtcNow,
					Email = "customer2@example.com",
					WeatherForecasts = new Collection<WeatherForecast>()
				},
				new Customer
				{
					Id = 3,
					CreatedAt = DateTimeOffset.UtcNow,
					ModifiedAt = DateTimeOffset.UtcNow,
					Email = "customer2@example.com",
					WeatherForecasts = new Collection<WeatherForecast>()
				},
				new Customer
				{
					Id = 4,
					CreatedAt = DateTimeOffset.UtcNow,
					ModifiedAt = DateTimeOffset.UtcNow,
					Email = "customer2@example.com",
					WeatherForecasts = new Collection<WeatherForecast>()
				}
			};
			var pagedCustomers = await PagedList<Customer>.Create(customers.BuildMock(), pageNumber, pageSize);
			repositoryMock.Read(pageNumber, pageSize, query).Returns(pagedCustomers);

			// Act
			var result = await customerService.Read(pageNumber, pageSize, query);

			// Assert
			Assert.NotNull(result);
			Assert.Equal(4, customers.Count);
			Assert.Equal(pageSize, result.Count);
		}

		[Fact]
		public async Task Read_CustomerEmail_ReturnsCustomer()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var customerEmail = new CustomerEmail("test@example.com");
			var expectedCustomer = new Customer { Email = customerEmail.Value };

			repositoryMock.Read(customerEmail).Returns(expectedCustomer);

			// Act
			var result = await customerService.Read(customerEmail);

			// Assert
			Assert.Equal(customerEmail.Value, result.Email);
		}

		[Fact]
		public async Task Update_ValidId_UpdateSuccessful()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var customerId = 1;
			var updateDto = new UpdateCustomerDto { };
			var existingCustomer = new Customer { };

			repositoryMock.Read(customerId).Returns(existingCustomer);

			// Act
			await customerService.Update(customerId, updateDto);

			// Assert
			// Verify that the repository's Update method was called
			await repositoryMock.Received().Update(Arg.Any<CustomerRequest>());
		}

		[Fact]
		public async Task Delete_ValidId_DeletionSuccessful()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var customerId = 1;
			var existingCustomer = new Customer { };

			repositoryMock.Read(customerId).Returns(existingCustomer);

			// Act
			await customerService.Delete(customerId);

			// Assert
			// Verify that the repository's Delete method was called
			await repositoryMock.Received().Delete(Arg.Any<int>());
		}

		[Fact]
		public async Task IsResourceOwner_ValidCustomerAndOrder_ReturnsTrue()
		{
			// Arrange
			var repositoryMock = Substitute.For<ICustomerRepository>();
			var customerService = new CustomerService(repositoryMock);

			var customerId = 1;
			var weatherForecastId = 1;
			repositoryMock.IsResourceOwner(Arg.Any<CustomerId>(), Arg.Any<WeatherForecastId>()).Returns(true);

			// Act
			var result = await customerService.IsResourceOwner(customerId, weatherForecastId);

			// Assert
			Assert.True(result);
		}
	}
}