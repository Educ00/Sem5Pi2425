using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.SurgeryRoomAggr;

namespace Sem5Pi2425.Infrastructure.SurgeryRoomInfra;

public class SurgeryRoomEntityTypeConfiguration : IEntityTypeConfiguration<SurgeryRoom> {
    public void Configure(EntityTypeBuilder<SurgeryRoom> builder) {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id).HasConversion(
            id => id.Value,
            value => new RoomNumber(int.Parse(value)));

        builder.Property(s => s.RoomCapacity).HasConversion(
                rc => rc.Value,
                value => new RoomCapacity(value))
            .IsRequired();

        builder.OwnsMany(s => s.AssignedEquipment);

        builder.Property(s => s.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Type)
            .IsRequired()
            .HasMaxLength(50);
    }
}