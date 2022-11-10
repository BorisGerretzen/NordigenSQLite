using Microsoft.EntityFrameworkCore;
using NordigenLib;
using NordigenLib.Models;
using NordigenLib.Models.API.Responses;
using NordigenService.EntityFramework;

namespace NordigenService;

public class Worker : BackgroundService {
    private readonly NordigenClient _client;
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly NordigenSettings _settings;

    public Worker(ILogger<Worker> logger, NordigenClient client, IServiceScopeFactory scopeFactory, NordigenSettings settings) {
        _logger = logger;
        _client = client;
        _scopeFactory = scopeFactory;
        _settings = settings;
    }

    public override Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogInformation("Starting database migration");
        using IServiceScope scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TransactionsContext>();
        context.Database.Migrate();
        _logger.LogInformation("Database migration finished");

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        JwtObtainResponse? jwtObtainResponse = null;

        while (!stoppingToken.IsCancellationRequested) {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            try {
                // Update token if required
                if (jwtObtainResponse == null || jwtObtainResponse.AccessExpiresDateTime > DateTime.Now.AddMinutes(30)) {
                    jwtObtainResponse = await UpdateJwt();
                }

                _logger.LogInformation($"JWT has {(int)Math.Round((jwtObtainResponse.AccessExpiresDateTime - DateTime.Now).TotalHours)} hours left.");

                // Get transactions
                var transactions = await GatherTransactions(jwtObtainResponse.AccessToken);

                // Filter transactions already in DB
                using IServiceScope scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<TransactionsContext>();
                var newTransactions = transactions.Transactions.Booked.Where(trans =>
                    !context.Transactions.Any(trans2 => trans2.TransactionId == trans.TransactionId)).ToList();
                _logger.LogInformation($"{newTransactions.Count} new transactions found.");

                // Save EF
                await context.AddRangeAsync(newTransactions, stoppingToken);
                await context.SaveChangesAsync(stoppingToken);
            }
            catch (Exception e) {
                _logger.LogCritical(e, "Exception occured.");
            }
            finally {
                await Wait(stoppingToken);
            }
        }
    }

    private async Task Wait(CancellationToken stoppingToken) {
        await Task.Delay(_settings.TimeOutMinutes * 60 * 1000, stoppingToken);
    }

    private async Task<JwtObtainResponse> UpdateJwt() {
        _logger.LogInformation("Obtaining new JWT.");
        return await _client.ObtainToken();
    }

    private async Task<TransactionsResponse> GatherTransactions(string token) {
        _logger.LogTrace("Gathering transactions.");
        var transactions = await _client.GetTransactions(token);
        _logger.LogInformation($"Received {transactions.Transactions.Booked.Count} transactions.");
        return transactions;
    }
}