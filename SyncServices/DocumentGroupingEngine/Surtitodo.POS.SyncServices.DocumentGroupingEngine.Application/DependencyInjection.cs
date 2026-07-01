using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Configuration;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.UseCases.GroupDocuments;
// NO agregar using de WorkerEventChannel ni AddSingleton aquí —
// el módulo ya lo registra antes de llamar AddApplication
// (mismo patrón aprendido con DocumentGroupingToSap)

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<GroupingSettings>(config.GetSection(GroupingSettings.SectionName));

            services.AddScoped<IGroupingOrchestrator, GroupDocumentsUseCase>();
            return services;
        }
    }
}