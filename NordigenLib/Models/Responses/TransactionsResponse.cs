using Newtonsoft.Json;
using NordigenLib.Models;

namespace NordigenLib.Responses;

[JsonObject]
public class TransactionsResponse {
    [JsonProperty("transactions")] public required Transactions Transactions { get; set; }
}