using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace NordigenLib.Models.API;

[JsonObject]
public class CreditorAccount {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [JsonIgnore]
    public required int CreditorAccountId { get; set; }

    [JsonProperty("iban")] public required string Iban { get; set; }
}