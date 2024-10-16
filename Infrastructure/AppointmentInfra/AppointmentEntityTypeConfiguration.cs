using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;

namespace Sem5Pi2425.Infrastructure.AppointmentInfra;

public class AppointmentEntityTypeConfiguration : IEntityTypeConfiguration<Appointment> {
    public void Configure(EntityTypeBuilder<Appointment> builder) {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .HasConversion(
                id => id.AsString(),
                value => new AppointmentId(value));

        builder.Property(a => a.Status)
            .HasConversion<string>();

        builder.Property(a => a.DateTime);

        builder.Property(a => a.RoomNumber)
            .HasConversion(
                rn => rn.Value,
                value => new RoomNumber(int.Parse(value)));

        builder.HasMany(a => a.AssignedStaffList)
            .WithOne()
            .HasForeignKey("AppointmentId");

        builder.HasOne(a => a.OperationRequest)
            .WithOne()
            .HasForeignKey<Appointment>("OperationRequestId")
            .IsRequired();
    }
}