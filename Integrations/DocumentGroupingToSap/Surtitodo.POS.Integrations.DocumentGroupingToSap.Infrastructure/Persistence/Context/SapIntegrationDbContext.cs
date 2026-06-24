using Microsoft.EntityFrameworkCore;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Context;

public sealed class SapIntegrationDbContext(DbContextOptions<SapIntegrationDbContext> options) : DbContext(options)
{
    public DbSet<DocumentAgroup> DocumentAgroups => Set<DocumentAgroup>();

    public DbSet<DocumentAgroupLine> DocumentAgroupLines => Set<DocumentAgroupLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SapIntegrationDbContext).Assembly);
    }
}
