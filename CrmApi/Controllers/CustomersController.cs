using CrmApi.Core.Models;
using CrmApi.Data;
using CrmApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CrmApi.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly CustomerRepository _repo;

    public CustomersController(CustomerRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _repo.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var customer = await _repo.GetByIdAsync(id);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? customerName, [FromQuery] string? sellerName)
    {
        if (!string.IsNullOrWhiteSpace(customerName))
            return Ok(await _repo.SearchByCustomerNameAsync(customerName));

        if (!string.IsNullOrWhiteSpace(sellerName))
            return Ok(await _repo.SearchBySellerNameAsync(sellerName));

        return BadRequest("Ange customerName eller sellerName som query-parameter.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ResponsibleSeller?.Name))
            return BadRequest("Ansvarig säljare måste anges.");

        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name,
            Title = dto.Title,
            Phone = dto.Phone,
            Email = dto.Email,
            Address = dto.Address,
            ResponsibleSeller = dto.ResponsibleSeller
        };

        var created = await _repo.CreateAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateCustomerDto dto)
    {
        var existing = await _repo.GetByIdAsync(id);
        if (existing is null)
            return NotFound();

        if (dto.Name is not null) existing.Name = dto.Name;
        if (dto.Title is not null) existing.Title = dto.Title;
        if (dto.Phone is not null) existing.Phone = dto.Phone;
        if (dto.Email is not null) existing.Email = dto.Email;
        if (dto.Address is not null) existing.Address = dto.Address;
        if (dto.ResponsibleSeller is not null) existing.ResponsibleSeller = dto.ResponsibleSeller;

        var updated = await _repo.UpdateAsync(id, existing);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _repo.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
