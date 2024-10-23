using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Infrastructure.PatientInfra;

public class PatientEntityConfiguration : IEntityTypeConfiguration<Patient> {
    public void Configure(EntityTypeBuilder<Patient> builder) {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasConversion(
            id => id.Value,
            value => new UserId(value));

        builder.HasOne(p => p.User);

        builder.OwnsOne(p => p.EmergencyContact, ec => {
            ec.Property(e => e.PhoneNumber).HasConversion(
                pn => pn.Value,
                value => new PhoneNumber(value));

            ec.Property(e => e.FullName).HasConversion(
                fn => fn.Value,
                value => new FullName(value));

            ec.Property(e => e.Email).HasConversion(
                e => e.Value,
                value => new Email(value));
        });

        builder.OwnsMany(p => p.MedicalConditions);

        builder.Property(p => p.BirthDate);
        
        builder.Property(p => p.Gender)
            .HasConversion<string>();

        builder.HasMany(p => p.AppointmentsHistory)
            .WithOne()
            .HasForeignKey("AppointmentId");
        
        builder.OwnsOne(p => p.MedicalRecordsNumber, ec => {
            ec.Property(e => e.Value);
        });

    }
}