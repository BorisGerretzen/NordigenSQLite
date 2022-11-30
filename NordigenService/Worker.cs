using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Nordigen.Net;
using Nordigen.Net.Queries;
using Nordigen.Net.Responses;
using NordigenLib.Models;
using NordigenService.EntityFramework;

namespace NordigenService;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly IMapper _mapper;
    private readonly INordigenApi _nordigen;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NordigenSettings _settings;

    private DateTime? _lastRetrieval;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory, INordigenApi nordigen, NordigenSettings settings, IMapper mapper) {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _nordigen = nordigen;
        _settings = settings;
        _mapper = mapper;
        _lastRetrieval = null;
    }

    public override async Task StartAsync(CancellationToken cancellationToken) {
        await Migrate();
        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        try {
            while (!stoppingToken.IsCancellationRequested) {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                var transactionsFilter = GetAccountTransactionsFilter(_settings.RetrievalMode);
                _lastRetrieval = DateTime.UtcNow;

                var dateFrom = transactionsFilter.DateFrom.HasValue ? transactionsFilter.DateFrom.Value.ToString("G") : "-";
                var dateTo = transactionsFilter.DateTo.HasValue ? transactionsFilter.DateTo.Value.ToString("G") : "-";
                _logger.LogInformation($"Getting transactions from {dateFrom} to {dateTo}.");

                var transactionsResponse = await _nordigen.Accounts.Transactions(_settings.AccountNumber, transactionsFilter, stoppingToken);
                transactionsResponse.Switch(
                    async transactions => await HandleTransactionsResponse(transactions, stoppingToken),
                    HandleErrorResponse
                );

                await Wait(stoppingToken);
            }
        }
        catch (Exception e) {
            _logger.LogCritical(e, "exception occured");
        }
    }

    private async Task Migrate() {
        _logger.LogInformation("Starting database migration");
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TransactionsContext>();
        await context.Database.MigrateAsync();
        await context.SaveChangesAsync();
        _logger.LogInformation("Database migration finished");
    }

    private AccountTransactionsFilter GetAccountTransactionsFilter(NordigenSettings.RetrievalOptions mode) => mode switch {
        NordigenSettings.RetrievalOptions.All => new AccountTransactionsFilter(null, null),
        NordigenSettings.RetrievalOptions.Range => new AccountTransactionsFilter(_settings.DateFrom, _settings.DateTo),
        NordigenSettings.RetrievalOptions.Dynamic when !_lastRetrieval.HasValue => new AccountTransactionsFilter(null, DateTime.UtcNow),
        NordigenSettings.RetrievalOptions.Dynamic when _lastRetrieval.HasValue => new AccountTransactionsFilter(_lastRetrieval, DateTime.UtcNow),
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown RetrievalMode found")
    };

    private async Task HandleTransactionsResponse(Transactions transactions, CancellationToken stoppingToken) {
        // Get EF
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TransactionsContext>();

        // Get only new transactions
        bool IdDoesNotExist(Transaction trans) => !context.Transactions.Any(trans2 => trans2.TransactionId == trans.TransactionId);
        var newTransactions = transactions.Booked
            .Where(IdDoesNotExist)
            .Select(transaction => _mapper.Map<TransactionEntity>(transaction))
            .ToList();
        _logger.LogInformation($"{newTransactions.Count} new transactions found.");


        // Write to db
        await context.AddRangeAsync(newTransactions, stoppingToken);
        await context.SaveChangesAsync(stoppingToken);
    }

    private void HandleErrorResponse(Error error) {
        _logger.LogError(string.IsNullOrEmpty(error.Detail) ? $"Received error code {error.StatusCode} from Nordigen." : error.Detail);
    }

    private async Task Wait(CancellationToken stoppingToken) {
        await Task.Delay(_settings.TimeOutMinutes * 60 * 1000, stoppingToken);
    }
}