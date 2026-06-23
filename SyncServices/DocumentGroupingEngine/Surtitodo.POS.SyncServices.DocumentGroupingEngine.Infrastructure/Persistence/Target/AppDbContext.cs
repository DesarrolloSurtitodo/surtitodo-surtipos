using Microsoft.EntityFrameworkCore;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<DocumentAgroup> DocumentAgroup => Set<DocumentAgroup>();
        public DbSet<DocumentAgroupLines> DocumentAgroupLines => Set<DocumentAgroupLines>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<DocumentAgroup>(e =>
            {
                e.ToTable("DocumentAgroup", "sap");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasMany(x => x.Lines)
                 .WithOne(x => x.DocumentAgroup)
                 .HasForeignKey(x => x.DocumentAgroupId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<DocumentAgroupLines>(e =>
            {
                e.ToTable("DocumentAgroupLines", "sap");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.Property(x => x.Price).HasPrecision(18, 2);
            });
        }
    }
}
