using Newtonsoft.Json;

namespace NordigenLib.Models;

[JsonObject]
public class CreditorAccount {
    [JsonProperty("iban")] public required string Iban { get; set; }
}