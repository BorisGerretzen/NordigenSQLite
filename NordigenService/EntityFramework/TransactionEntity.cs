using System.ComponentModel.DataAnnotations;

namespace NordigenService.EntityFramework;

public class TransactionEntity {
    public TransactionEntity(string? transactionId,
        string endToEndId,
        DateTime bookingDate,
        string amount,
        string? currency,
        string remittanceInformationUnstructured, string? creditorName, string? creditorIban, string? creditorBban, string? debtorName, string? debtorIban, string? debtorBban) {
        TransactionId = transactionId;
        EndToEndId = endToEndId;
        BookingDate = bookingDate;
        Amount = amount;
        Currency = currency;
        RemittanceInformationUnstructured = remittanceInformationUnstructured;
        CreditorName = creditorName;
        CreditorIban = creditorIban;
        CreditorBban = creditorBban;
        DebtorName = debtorName;
        DebtorIban = debtorIban;
        DebtorBban = debtorBban;
    }

    private TransactionEntity() { }

    [Key] public string? TransactionId { get; private set; }
    public string? EndToEndId { get; private set; }
    public DateTime BookingDate { get; private set; }
    public string? CreditorName { get; private set; }
    public string? CreditorIban { get; private set; }
    public string? CreditorBban { get; private set; }

    public string? DebtorName { get; private set; }
    public string? DebtorIban { get; private set; }
    public string? DebtorBban { get; private set; }

    public string Amount { get; private set; }
    public string? Currency { get; private set; }
    public string? RemittanceInformationUnstructured { get; private set; }
}