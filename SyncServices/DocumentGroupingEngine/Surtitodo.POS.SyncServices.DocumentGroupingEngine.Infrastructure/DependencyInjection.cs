using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Source.Repository;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target.Repository;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Pipeline;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Services;
using System.Data;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // ── Target: EF Core ──────────────────────────────────────────
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("TargetConnectionDb")));

            services.AddScoped<IGroupedDocumentRepository, GroupedDocumentRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Source: Dapper ───────────────────────────────────────────
            services.AddScoped<IDbConnection>(_ => new SqlConnection(config.GetConnectionString("SourceConnectionDb")));

            services.AddScoped<IDocumentsRepository, DocumentsRepository>();
            services.AddScoped<IDocumentsLinesRepository, DocumentsLinesRepository>();

            // ── Transversales ────────────────────────────────────────────
            services.AddScoped<IErrorLogService, ErrorLogService>();
            services.AddScoped<GroupingPipelineBehavior>();

            return services;
        }
     }
}
