using Newtonsoft.Json;

namespace CrmApi.Core.Models;

public class Seller
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("phone")]
    public string Phone { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
