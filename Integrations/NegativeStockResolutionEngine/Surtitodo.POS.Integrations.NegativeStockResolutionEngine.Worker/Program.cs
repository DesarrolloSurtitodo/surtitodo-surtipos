using Surtitodo.POS.Integrations.NegativeStockResolutionEngine.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
