using Serilog;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker;

var logsPath = Path.Combine(AppContext.BaseDirectory, "Logs");
Directory.CreateDirectory(Path.Combine(logsPath, "Requests"));
Directory.CreateDirectory(Path.Combine(logsPath, "Responses"));

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, lc) => lc
        .ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console()
        .WriteTo.File(
            path: Path.Combine(logsPath, "worker-.log"),
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .ConfigureServices((ctx, services) =>
    {
        services
            .AddApplication(ctx.Configuration)
            .AddInfrastructure(ctx.Configuration);

        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();