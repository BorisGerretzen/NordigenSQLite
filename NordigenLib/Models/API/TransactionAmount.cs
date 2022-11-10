using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NordigenLib.Models.API;

[JsonObject]
public class TransactionAmount {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public required int TransactionAmountId { get; set; }

    [JsonProperty("amount")] public required double Amount { get; set; }
    [JsonProperty("currency")] public required string Currency { get; set; }
}