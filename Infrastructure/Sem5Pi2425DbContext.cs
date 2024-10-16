using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Infrastructure.AppointmentInfra;
using Sem5Pi2425.Infrastructure.OperationRequestInfra;
using Sem5Pi2425.Infrastructure.OperationTypeInfra;
using Sem5Pi2425.Infrastructure.PatientInfra;
using Sem5Pi2425.Infrastructure.StaffInfra;
using Sem5Pi2425.Infrastructure.SurgeryRoomInfra;
using Sem5Pi2425.Infrastructure.SystemUser;

namespace Sem5Pi2425.Infrastructure
{
    public class Sem5Pi2425DbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<OperationRequest> OperationRequests { get; set; }
        public DbSet<OperationType> OperationTypes { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<SurgeryRoom> SurgeryRooms { get; set; }
        
        public Sem5Pi2425DbContext(DbContextOptions options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationRequestTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OperationTypeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PatientEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StaffEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SurgeryRoomEntityTypeConfiguration());
        }
    }
}