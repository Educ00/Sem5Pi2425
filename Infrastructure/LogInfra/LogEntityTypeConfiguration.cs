using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.LogAggr;

namespace Sem5Pi2425.Infrastructure.LogInfra;

public class LogEntityTypeConfiguration : IEntityTypeConfiguration<Log> {
    public void Configure(EntityTypeBuilder<Log> builder) {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .HasConversion(
                l => l.Value,
                value => new LogId(value));

        builder.Property(l => l.Type);

        builder.Property(l => l.Timestamp);
    }
}