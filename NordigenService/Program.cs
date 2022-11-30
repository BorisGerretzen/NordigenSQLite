using System.Diagnostics;
using System.Net;
using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Nordigen.Net;
using Nordigen.Net.Responses;
using NordigenService;
using NordigenService.EntityFramework;
using Polly;
using Polly.Extensions.Http;

const string serviceName = "NordigenService";

void Run() {
    var host = Host
        .CreateDefaultBuilder(args)
        .ConfigureServices((ctx, services) => {
            services.AddDbContext<TransactionsContext>();

            // Get config
            var config = ctx.Configuration;
            var options = config.GetSection("Nordigen").Get<NordigenSettings>();
            if (options == null) throw new JsonException("Invalid appsettings.json.");
            services.AddSingleton(options);

            services.AddNordigenApi()
                .AddPolicyHandler(
                    HttpPolicyExtensions
                        .HandleTransientHttpError()
                        .OrResult(msg => msg.StatusCode == HttpStatusCode.ServiceUnavailable)
                        .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

            var mapper = new MapperConfiguration(cfg => {
                // cfg.CreateMap<AccountIdentification, TransactionEntity>()
                //     .ForPath(dest => dest.CreditorIban, opt => opt.MapFrom(s => s.))
                cfg.CreateMap<TransactionEntity, Transaction>()
                    .ForMember(dest => dest.CreditorName, opt => opt.MapFrom(s => s.CreditorName))
                    .ForPath(dest => dest.CreditorAccount.IBAN, opt => opt.MapFrom(s => s.CreditorIban))
                    .ForPath(dest => dest.CreditorAccount.BBAN, opt => opt.MapFrom(s => s.CreditorBban))
                    .ForMember(dest => dest.DebtorName, opt => opt.MapFrom(s => s.DebtorName))
                    .ForPath(dest => dest.DebtorAccount.IBAN, opt => opt.MapFrom(s => s.DebtorIban))
                    .ForPath(dest => dest.DebtorAccount.BBAN, opt => opt.MapFrom(s => s.DebtorBban))
                    .ForPath(dest => dest.TransactionAmount.Amount, opt => opt.MapFrom(s => s.Amount))
                    .ForPath(dest => dest.TransactionAmount.Currency, opt => opt.MapFrom(s => s.Currency))
                    .ReverseMap()
                    .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(s => string.IsNullOrEmpty(s.TransactionId) ? s.InternalTransactionId : s.TransactionId));

                cfg.CreateMap<TransactionAmount, TransactionEntity>()
                    .ForMember(dest => dest.Amount,
                        opt => opt.MapFrom(s => s.Amount))
                    .ForMember(dest => dest.Currency,
                        opt => opt.MapFrom(s => s.Currency));
            }).CreateMapper();
            services.AddSingleton(mapper);
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
            services.AddHostedService<Worker>();
        })
        .UseWindowsService()
        .Build();
    host.Run();
}

void Install() {
    var exePath = Process.GetCurrentProcess().MainModule.FileName;
    var output = "";
    output += RunCommand($"sc.exe create {serviceName} binpath={exePath} start=auto");
    output += RunCommand($"sc.exe failure {serviceName} actions=restart/1000/restart/2000/restart/10000 reset=120");
    Console.WriteLine(output);
}

void Uninstall() {
    var output = "";
    output += RunCommand($"sc.exe delete {serviceName}");
    Console.WriteLine(output);
}

string RunCommand(string command) {
    var process = new Process();
    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
    process.StartInfo.FileName = "cmd.exe";
    process.StartInfo.Arguments = @$"/C {command}";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;
    process.StartInfo.RedirectStandardOutput = true;
    process.Start();
    var strOutput = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    return strOutput;
}

if (args.Length > 0) {
    switch (args[0]) {
        case "--install":
            Install();
            return;
        case "--uninstall":
            Uninstall();
            return;
        case "--help":
            Console.WriteLine("--install            Installs the application as a Windows service");
            Console.WriteLine("--uninstall          Uninstalls the Windows service");
            Console.WriteLine("--help               Displays this information");
            return;
    }
}

Run();