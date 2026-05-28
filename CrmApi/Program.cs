using CrmApi.Data;
using CrmApi.DTO;
using CrmApi.Services;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "CRM API", Version = "v1" });
});

builder.Services.AddSingleton<CosmosClient>(_ =>
    new CosmosClient(builder.Configuration["CosmosDb:ConnectionString"]));

builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var customers = app.MapGroup("/api/customers");

customers.MapGet("/", async (ICustomerService service) =>
    Results.Ok(await service.GetAllAsync()));

customers.MapGet("/search", async (string? customerName, string? sellerName, ICustomerService service) =>
{
    if (!string.IsNullOrWhiteSpace(customerName))
        return Results.Ok(await service.SearchByCustomerNameAsync(customerName));

    if (!string.IsNullOrWhiteSpace(sellerName))
        return Results.Ok(await service.SearchBySellerNameAsync(sellerName));

    return Results.BadRequest("Ange customerName eller sellerName som query-parameter.");
});

customers.MapGet("/{id}", async (string id, ICustomerService service) =>
{
    var customer = await service.GetByIdAsync(id);
    return customer is null ? Results.NotFound() : Results.Ok(customer);
});

customers.MapPost("/", async (CreateCustomerDto dto, ICustomerService service) =>
{
    if (string.IsNullOrWhiteSpace(dto.ResponsibleSeller?.Name))
        return Results.BadRequest("Ansvarig säljare måste anges.");

    var created = await service.CreateAsync(dto);
    return Results.Created($"/api/customers/{created.Id}", created);
});

customers.MapPut("/{id}", async (string id, UpdateCustomerDto dto, ICustomerService service) =>
{
    var updated = await service.UpdateAsync(id, dto);
    return updated is null ? Results.NotFound() : Results.Ok(updated);
});

customers.MapDelete("/{id}", async (string id, ICustomerService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();
