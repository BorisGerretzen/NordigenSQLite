using NordigenLib;
using NordigenLib.Responses;

namespace NordigenService;

public class Worker : BackgroundService {
    private readonly NordigenClient _client;
    private readonly ILogger<Worker> _logger;

    private JwtObtainResponse? _jwtObtainResponse;


    public Worker(ILogger<Worker> logger, NordigenClient client) {
        _logger = logger;
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            // Update token if required
            if (_jwtObtainResponse == null || _jwtObtainResponse.AccessExpiresDateTime > DateTime.Now.AddMinutes(30)) await UpdateJwt();
            if (_jwtObtainResponse != null) {
                var trans = await _client.GetTransactions(_jwtObtainResponse.AccessToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }

    private async Task UpdateJwt() {
        _logger.LogInformation("Obtaining new JWT.");
        try {
            _jwtObtainResponse = await _client.ObtainToken();
        }
        catch (Exception e) {
            _logger.LogCritical(e, "Exception occured while obtaining JWT.");
        }
    }
}