using Microsoft.EntityFrameworkCore;
using NordigenLib.Models.API;

namespace NordigenService.EntityFramework;

public class TransactionsContext : DbContext {
    public TransactionsContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "nordigen.db");
    }

    public required DbSet<Transaction> Transactions { get; set; }
    public required DbSet<CreditorAccount> CreditorAccounts { get; set; }
    public required DbSet<TransactionAmount> TransactionAmounts { get; set; }

    public string DbPath { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}