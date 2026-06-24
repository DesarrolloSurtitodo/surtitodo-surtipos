using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Services;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Context;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Repositories;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Services;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<SapIntegrationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDocumentAgroupRepository, DocumentAgroupRepository>();
            services.AddScoped<ISapInvoiceLookupRepository, SapInvoiceLookupRepository>();
            services.AddScoped<ISapInvoiceService, SapInvoiceService>();

            services.AddHttpClient(
                "SapServiceLayer",
                client =>
                {
                    client.BaseAddress =
                        new Uri(configuration["Sap:BaseUrl"]!);
                });

            services.AddSingleton<ISapSessionManager, SapSessionManager>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            return services;
        }
    }
}
