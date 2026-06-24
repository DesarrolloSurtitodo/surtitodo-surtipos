using Serilog;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure;

using Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker;
var builder = Host.CreateApplicationBuilder(args);

// ── Serilog ──────────────────────────────────────────────────────────
builder.Logging.ClearProviders();
builder.Services.AddSerilog((ctx, lc) => lc.ReadFrom.Configuration(builder.Configuration));

// ── Capas ────────────────────────────────────────────────────────────
builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

// ── Worker ───────────────────────────────────────────────────────────
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();