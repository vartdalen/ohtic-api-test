using Microsoft.EntityFrameworkCore;
using Ohtic.Test.Data;
using Ohtic.Test.Data.Abstractions.Repositories;
using Ohtic.Test.Data.Collections;
using Ohtic.Test.Data.Extensions;
using Ohtic.Test.Data.Models.Entities;
using Ohtic.Test.Data.Models.Indexes;
using Ohtic.Test.Data.Models.Requests;

public class CustomerRepository : ICustomerRepository
{
    private readonly WeatherForecastsDbContext _context;

    public CustomerRepository(WeatherForecastsDbContext context)
    {
        _context = context;
    }

    public async Task<Customer> Create(CustomerRequest request)
    {
        var result = await _context.Customers.AddAsync(request.Customer);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<Customer?> Read(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task<PagedList<Customer>> Read(
        int pageNumber,
        int pageSize,
        string? q
    )
    {
        var customers = _context.Customers.Conditional(
            !q.IsNullOrEmpty(),
            x => x.Where(c =>
                c.Email.Contains(q!)
            )
        );
        return await PagedList<Customer>.Create(customers, pageNumber, pageSize);
    }

    public async Task<Customer?> Read(CustomerEmail customerEmail)
    {
        return await _context.Customers.SingleOrDefaultAsync(c => c.Email == customerEmail.Value);
    }

    public async Task Update(CustomerRequest request)
    {
        _context.Customers.Update(request.Customer);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer is not null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsResourceOwner(CustomerId customerId, WeatherForecastId weatherForecastId)
    {
        return await _context.WeatherForecasts
            .AnyAsync(wf => wf.Id == weatherForecastId.Value && wf.CustomerId == customerId.Value);
    }
}