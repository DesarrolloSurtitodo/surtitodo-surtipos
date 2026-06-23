using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.UseCases.GroupDocuments;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IGroupingOrchestrator, GroupDocumentsUseCase>();
            return services;
        }
    }
}
