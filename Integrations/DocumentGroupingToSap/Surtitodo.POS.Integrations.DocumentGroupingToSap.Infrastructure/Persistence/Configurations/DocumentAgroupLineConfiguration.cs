using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Configurations;

public sealed class DocumentAgroupLineConfiguration : IEntityTypeConfiguration<DocumentAgroupLine>
{
    public void Configure(EntityTypeBuilder<DocumentAgroupLine> builder)
    {
        builder.ToTable("DocumentAgroupLines", "sap");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.HasOne(x => x.DocumentAgroup)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.DocumentAgroupId);
       
    }
}
