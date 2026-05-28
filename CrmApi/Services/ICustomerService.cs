using CrmApi.Core.Models;
using CrmApi.DTO;

namespace CrmApi.Services;

public interface ICustomerService
{
    Task<IEnumerable<Customer>> GetAllAsync();
    Task<Customer?> GetByIdAsync(string id);
    Task<IEnumerable<Customer>> SearchByCustomerNameAsync(string name);
    Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName);
    Task<Customer> CreateAsync(CreateCustomerDto dto);
    Task<Customer?> UpdateAsync(string id, UpdateCustomerDto dto);
    Task<bool> DeleteAsync(string id);
}
