﻿using Newtonsoft.Json;

namespace NordigenLib.Models;

[JsonObject]
public class Transaction {
    [JsonProperty("transactionId")] public required string TransactionId { get; set; }
    [JsonProperty("endToEndId")] public required string EndToEndId { get; set; }
    [JsonProperty("bookingDate")] public required DateTime BookingDate { get; set; }
    [JsonProperty("transactionAmount")] public required TransactionAmount TransactionAmount { get; set; }
    [JsonProperty("creditorName")] public required string CreditorName { get; set; }
    [JsonProperty("creditorAccount")] public CreditorAccount? CreditorAccount { get; set; }

    [JsonProperty("remittanceInformationUnstructured")]
    public required string RemittanceInformationUnstructured { get; set; }

    [JsonProperty("proprietaryBankTransactionCode")]
    public required string ProprietaryBankTransactionCode { get; set; }

    [JsonProperty("internalTransactionId")]
    public required string InternalTransactionId { get; set; }
}