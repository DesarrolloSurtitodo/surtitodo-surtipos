using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Persistence.Configurations;

public sealed class DocumentAgroupConfiguration : IEntityTypeConfiguration<DocumentAgroup>
{
    public void Configure(EntityTypeBuilder<DocumentAgroup> builder)
    {
        builder.ToTable("DocumentAgroup", "sap");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.NumAtCard)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CardCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(x => x.IntegrationStatus)
            .HasMaxLength(1)
            .IsRequired();
    }
}
