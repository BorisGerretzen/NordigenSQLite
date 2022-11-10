namespace NordigenLib.Models;

public class NordigenSettings {
    /// <summary>
    /// Nordigen Secret ID, I know not very secure...
    /// </summary>
    public required string SecretId { get; set; }

    /// <summary>
    /// Nordigen secret key, also not very secure.
    /// </summary>
    public required string SecretKey { get; set; }

    /// <summary>
    /// Nordigen account ID.
    /// </summary>
    public required string AccountNumber { get; set; }

    /// <summary>
    /// Number of minutes to wait between each transaction retrieval
    /// </summary>
    public required int TimeOutMinutes { get; set; } = 60;

    /// <summary>
    /// Base url to the nordigen api. No trailing / required.
    /// </summary>
    public required string BaseUrl { get; set; } = "https://ob.nordigen.com";

    /// <summary>
    /// Relative URL to the endpoint to generate a JWT token.
    /// </summary>
    public required string UrlJwtObtain { get; set; } = "/api/v2/token/new/";

    /// <summary>
    /// Relative URL to the endpoint to refresh a JWT token.
    /// </summary>
    public required string UrlJwtRefresh { get; set; } = "/api/v2/token/refresh/";

    /// <summary>
    /// Relative URL to the endpoint to get the transactions.
    /// </summary>
    public required string UrlGetTransactions { get; set; } = "/api/v2/accounts/:id/transactions/";

    /// <summary>
    /// Optional, date to get transactions from.
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Optional, date to get transactions to.
    /// </summary>
    public DateTime? DateTo { get; set; }
}