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
            LeaseContainerName = "leases-v2",
            LeaseDatabaseName = "CrmDb",
            CreateLeaseContainerIfNotExists = true,
            StartFromBeginning = true
        )] IReadOnlyList<Customer> customers)
    {
        _logger.LogInformation("Cosmos DB change feed: {Count} kund(er) ändrade.", customers.Count);

        foreach (var customer in customers)
        {
            _logger.LogInformation("Behandlar kund: {Name} ({Id})", customer.Name, customer.Id);

            try
            {
                if (string.IsNullOrWhiteSpace(customer.ResponsibleSeller?.Email))
                {
                    _logger.LogWarning("Kund {Id} saknar säljarens e-postadress – hoppar över.", customer.Id);
                    continue;
                }

                if (customer.IsNew)
                {
                    _logger.LogInformation("Skickar ny-kund-mail för {Name}", customer.Name);
                    await _emailService.SendNewCustomerNotificationAsync(
                        sellerEmail: customer.ResponsibleSeller.Email,
                        sellerName: customer.ResponsibleSeller.Name,
                        customerName: customer.Name,
                        customerEmail: customer.Email,
                        customerPhone: customer.Phone,
                        customerAddress: customer.Address,
                        customerTitle: customer.Title
                    );
                }
                else
                {
                    _logger.LogInformation("Skickar uppdaterings-mail för {Name}", customer.Name);
                    await _emailService.SendUpdatedCustomerNotificationAsync(
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fel vid e-postutskick för kund {Id} ({Name})", customer.Id, customer.Name);
            }
        }
    }
}
