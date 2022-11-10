using Newtonsoft.Json;

namespace NordigenLib.Models;

[JsonObject]
public class TransactionAmount {
    [JsonProperty("amount")] public required double Amount { get; set; }
    [JsonProperty("currency")] public required string Currency { get; set; }
}