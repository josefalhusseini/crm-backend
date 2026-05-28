using CrmApi.Core.Models;
using CrmApi.Data;
using CrmApi.DTO;

namespace CrmApi.Services;

public class CustomerService : ICustomerService
{
    private readonly CustomerRepository _repo;

    public CustomerService(CustomerRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Customer>> GetAllAsync() =>
        _repo.GetAllAsync();

    public Task<Customer?> GetByIdAsync(string id) =>
        _repo.GetByIdAsync(id);

    public Task<IEnumerable<Customer>> SearchByCustomerNameAsync(string name) =>
        _repo.SearchByCustomerNameAsync(name);

    public Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName) =>
        _repo.SearchBySellerNameAsync(sellerName);

    public async Task<Customer> CreateAsync(CreateCustomerDto dto)
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Title = dto.Title,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            ResponsibleSeller = dto.ResponsibleSeller,
            IsNew = true
        };

        return await _repo.CreateAsync(customer);
    }

    public async Task<Customer?> UpdateAsync(string id, UpdateCustomerDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null) return null;

        if (dto.Name is not null) existing.Name = dto.Name;
        if (dto.Title is not null) existing.Title = dto.Title;
        if (dto.Phone is not null) existing.Phone = dto.Phone;
        if (dto.Email is not null) existing.Email = dto.Email;
        if (dto.Address is not null) existing.Address = dto.Address;
        if (dto.ResponsibleSeller is not null) existing.ResponsibleSeller = dto.ResponsibleSeller;
        existing.IsNew = false;

        return await _repo.UpdateAsync(id, existing);
    }

    public Task<bool> DeleteAsync(string id) =>
        _repo.DeleteAsync(id);
}
