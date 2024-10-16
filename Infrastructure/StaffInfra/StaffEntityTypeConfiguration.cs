using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.StaffAggr;

namespace Sem5Pi2425.Infrastructure.StaffInfra;

public class StaffEntityTypeConfiguration : IEntityTypeConfiguration<Staff> {
    public void Configure(EntityTypeBuilder<Staff> builder) {
        builder.HasKey(s => s.Id);

        builder.Property(u => u.Id).HasConversion(
            id => id.AsString(),
            value => new UniqueIdentifier(value));

        builder.HasOne(s => s.User);

        builder.OwnsMany(s => s.AvailableSlots);

        builder.Property(s => s.Specialization)
            .HasConversion<string>();
    }
}