using CrmFunction.Models;
using CrmFunction.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace CrmFunction;

public class CustomerChangeFeed
{
    private readonly EmailService _emailService;
    private readonly ILogger<CustomerChangeFeed> _logger;

    public CustomerChangeFeed(EmailService emailService, ILogger<CustomerChangeFeed> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    [Function(nameof(CustomerChangeFeed))]
    public async Task Run(
        [CosmosDBTrigger(
            databaseName: "CrmDb",
            containerName: "Customers",
            Connection = "CosmosDbConnectionString",
            LeaseContainerName = "leases",
            CreateLeaseContainerIfNotExists = false
        )] IReadOnlyList<Customer> customers)
    {
        _logger.LogInformation("Cosmos DB change feed: {Count} kund(er) ändrade.", customers.Count);

        foreach (var customer in customers)
        {
            _logger.LogInformation("Behandlar kund: {Name} ({Id})", customer.Name, customer.Id);

            await _emailService.SendSellerNotificationAsync(
                sellerEmail: customer.ResponsibleSeller.Email,
                sellerName: customer.ResponsibleSeller.Name,
                customerName: customer.Name,
                customerEmail: customer.Email,
                customerPhone: customer.Phone,
                customerAddress: customer.Address,
                customerTitle: customer.Title
            );
        }
    }
}
