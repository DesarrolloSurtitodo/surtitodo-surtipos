using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.UseCases;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.UseCases;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SapOptions>(configuration.GetSection(SapOptions.SectionName));
        services.Configure<IntegrationOptions>(configuration.GetSection(IntegrationOptions.SectionName));
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.Configure<HanaOptions>(configuration.GetSection(HanaOptions.SectionName));

        services.AddScoped<IProcessPendingDocumentsUseCase, ProcessPendingDocumentsUseCase>();

        return services;
    }
}
