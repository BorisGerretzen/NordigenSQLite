namespace NordigenLib.Models;

public class NordigenSettings {
    public enum RetrievalOptions {
        /// <summary>
        /// Retrieve all possible transactions every time.
        /// </summary>
        All,

        /// <summary>
        /// Retrieve only the transactions in the specified date range.
        /// </summary>
        Range,

        /// <summary>
        /// Retrieve all transactions on first launch, then only the ones since the last retrieval.
        /// </summary>
        Dynamic
    }

    /// <summary>
    /// Nordigen account ID.
    /// </summary>
    public required string AccountNumber { get; set; }

    /// <summary>
    /// Number of minutes to wait between each transaction retrieval
    /// </summary>
    public required int TimeOutMinutes { get; set; } = 60;

    /// <summary>
    /// Optional, date to get transactions from.
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Optional, date to get transactions to.
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// How to retrieve the transactions.
    /// </summary>
    public RetrievalOptions RetrievalMode { get; set; }
}