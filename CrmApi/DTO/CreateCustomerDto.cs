using CrmApi.Core.Models;

namespace CrmApi.DTO;

public class CreateCustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Seller ResponsibleSeller { get; set; } = new();
}
