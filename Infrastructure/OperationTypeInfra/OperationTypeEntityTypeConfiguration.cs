using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.OperationTypeAggr;

namespace Sem5Pi2425.Infrastructure.OperationTypeInfra;

public class OperationTypeEntityTypeConfiguration : IEntityTypeConfiguration<OperationType> {
    public void Configure(EntityTypeBuilder<OperationType> builder) {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.AsString(),
                value => new OperationTypeId(value));

        builder.Property(o => o.Duration);

        builder.Property(o => o.Name)
            .HasConversion(
                name => name.Value,
                value => new Name(value))
            .HasMaxLength(100);
        
        builder.Property(o => o.Description)
            .HasConversion(
                desc => desc.Value,
                value => new Description(value))
            .HasMaxLength(500);

        builder.Property(o => o.NeededSpecializations)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

    }
}