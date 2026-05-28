using Newtonsoft.Json;

namespace CrmApi.Core.Models;

public class Customer
{
    [JsonProperty("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("address")]
    public string Address { get; set; } = string.Empty;

    [JsonProperty("responsibleSeller")]
    public Seller ResponsibleSeller { get; set; } = new();

    [JsonProperty("isNew")]
    public bool IsNew { get; set; } = false;
}
