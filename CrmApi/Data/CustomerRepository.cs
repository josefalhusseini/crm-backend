using CrmApi.Core.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;

namespace CrmApi.Data;

public class CustomerRepository
{
    private readonly Container _container;

    public CustomerRepository(CosmosClient client, IConfiguration config)
    {
        var db = client.GetDatabase(config["CosmosDb:DatabaseName"]);
        _container = db.GetContainer(config["CosmosDb:ContainerName"]);
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var query = _container.GetItemLinqQueryable<Customer>().ToFeedIterator();
        var results = new List<Customer>();
        while (query.HasMoreResults)
            results.AddRange(await query.ReadNextAsync());
        return results;
    }

    public async Task<Customer?> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<Customer>(id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<IEnumerable<Customer>> SearchByCustomerNameAsync(string name)
    {
        var lower = name.ToLower();
        var query = _container
            .GetItemLinqQueryable<Customer>()
            .Where(c => c.Name.ToLower().Contains(lower))
            .ToFeedIterator();
        var results = new List<Customer>();
        while (query.HasMoreResults)
            results.AddRange(await query.ReadNextAsync());
        return results;
    }

    public async Task<IEnumerable<Customer>> SearchBySellerNameAsync(string sellerName)
    {
        var lower = sellerName.ToLower();
        var query = _container
            .GetItemLinqQueryable<Customer>()
            .Where(c => c.ResponsibleSeller.Name.ToLower().Contains(lower))
            .ToFeedIterator();
        var results = new List<Customer>();
        while (query.HasMoreResults)
            results.AddRange(await query.ReadNextAsync());
        return results;
    }

    public async Task<Customer> CreateAsync(Customer customer)
    {
        var response = await _container.CreateItemAsync(customer, new PartitionKey(customer.Id));
        return response.Resource;
    }

    public async Task<Customer?> UpdateAsync(string id, Customer customer)
    {
        try
        {
            customer.Id = id;
            var response = await _container.ReplaceItemAsync(customer, id, new PartitionKey(id));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<Customer>(id, new PartitionKey(id));
            return true;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }
    }
}
