using Microsoft.EntityFrameworkCore;

namespace NordigenService.EntityFramework;

public class TransactionsContext : DbContext {
    public TransactionsContext() {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.ProgramFiles;
        var path = Environment.GetFolderPath(folder);
        var folderPath = Path.Join(path, "NordigenService");
        Directory.CreateDirectory(folderPath);
        DbPath = Path.Join(folderPath, "nordigen.db");
    }

    public required DbSet<TransactionEntity> Transactions { get; set; }

    public string DbPath { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}