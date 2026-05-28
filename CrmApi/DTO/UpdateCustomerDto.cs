using System.ComponentModel.DataAnnotations;
using CrmApi.Core.Models;

namespace CrmApi.DTO;

public class UpdateCustomerDto
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Phone { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    public string? Address { get; set; }
    public Seller? ResponsibleSeller { get; set; }
}
