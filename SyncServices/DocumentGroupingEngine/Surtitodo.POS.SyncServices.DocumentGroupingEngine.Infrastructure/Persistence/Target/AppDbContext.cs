using Microsoft.EntityFrameworkCore;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Infrastructure.Persistence.Target
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<DocumentAgroup> DocumentAgroups => Set<DocumentAgroup>();
        public DbSet<DocumentAgroupLines> DocumentAgroupLines => Set<DocumentAgroupLines>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<DocumentAgroup>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
                e.HasMany(x => x.Lines)
                 .WithOne(x => x.DocumentAgroup)
                 .HasForeignKey(x => x.DocumentAgroupId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            mb.Entity<DocumentAgroupLines>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
