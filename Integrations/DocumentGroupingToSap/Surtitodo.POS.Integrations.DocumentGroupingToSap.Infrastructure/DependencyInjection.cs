using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Files;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Services;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Files;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Context;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Services;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sapOptions = configuration.GetSection("Sap").Get<SapOptions>()!;
        var integrationOptions = configuration.GetSection("Integration").Get<IntegrationOptions>()!;

        services.AddDbContext<SapIntegrationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IDocumentAgroupRepository, DocumentAgroupRepository>();
        services.AddScoped<ISapInvoiceLookupRepository, SapInvoiceLookupRepository>();
        services.AddScoped<ISapInvoiceService, SapInvoiceService>();
        services.AddScoped<IJsonFileStorage, JsonFileStorage>();

        services.AddHttpClient("SapServiceLayer", client =>
        {
            client.BaseAddress = new Uri(sapOptions.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(integrationOptions.HttpTimeoutSeconds);
        })
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = sapOptions.IgnoreSslErrors
                ? HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                : null
        });

        services.AddSingleton<ISapSessionManager, SapSessionManager>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
