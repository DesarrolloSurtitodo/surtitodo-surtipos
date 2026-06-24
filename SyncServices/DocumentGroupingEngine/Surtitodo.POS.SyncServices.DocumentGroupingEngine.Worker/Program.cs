// Program.cs
using Serilog;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Worker;

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