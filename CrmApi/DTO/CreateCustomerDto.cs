using System.ComponentModel.DataAnnotations;
using CrmApi.Core.Models;

namespace CrmApi.DTO;

public class CreateCustomerDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    [Required]
    public Seller ResponsibleSeller { get; set; } = new();
}
