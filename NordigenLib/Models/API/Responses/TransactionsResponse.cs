using Newtonsoft.Json;

namespace NordigenLib.Models.API.Responses;

[JsonObject]
public class TransactionsResponse {
    [JsonProperty("transactions")] public required Transactions Transactions { get; set; }
}