﻿using Newtonsoft.Json;

namespace NordigenLib.Models;

[JsonObject]
public class Transactions {
    [JsonProperty("booked")] public List<Transaction> Booked { get; set; } = new();
    [JsonProperty("pending")] public List<Transaction> Pending { get; set; } = new();
}