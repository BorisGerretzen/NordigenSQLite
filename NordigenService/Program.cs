using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using NordigenLib;
using NordigenService;

IHost host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) => {
        // Get config
        IConfiguration config = ctx.Configuration;
        NordigenSettings? options = config.GetSection("Nordigen").Get<NordigenSettings>();
        if (options == null) throw new JsonException("Invalid appsettings.json.");
        services.AddSingleton(options);

        services.AddSingleton<MemoryCache>();
        services.AddSingleton<NordigenClient>();
        services.AddHttpClient<NordigenClient>(client => client.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute));
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();