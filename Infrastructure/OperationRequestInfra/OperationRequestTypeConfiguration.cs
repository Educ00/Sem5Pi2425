using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.OperationRequestAggr;

namespace Sem5Pi2425.Infrastructure.OperationRequestInfra;

public class OperationRequestTypeConfiguration : IEntityTypeConfiguration<OperationRequest> {
    public void Configure(EntityTypeBuilder<OperationRequest> builder) {
        builder.HasKey(o => o.Id);

        // Configure the Id property
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.AsString(),
                value => new OperationRequestId(value));

        // Configure the Deadline property
        builder.Property(o => o.Deadline);

        // Configure the Priority property
        builder.Property(o => o.Priority)
            .HasConversion<string>();

        // Configure the relationship with Doctor (User)
        builder.HasOne(o => o.Doctor)
            .WithMany()
            .HasForeignKey("DoctorId")
            .IsRequired();

        // Configure the relationship with Patient
        builder.HasOne(o => o.Patient)
            .WithMany()
            .HasForeignKey("PatientId")
            .IsRequired();
    }
}