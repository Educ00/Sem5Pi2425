using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Infrastructure.BootstrapInfra
{
    public class UserBootstrapService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserBootstrapService(IUserRepository userRepository, IPatientRepository patientRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task BootstrapInitialUsers()
        {
            if (await _userRepository.GetAllAsync() is { Count: > 0 }) return;

            await CreateAdminUser();
            await CreateDoctorUser();
            await CreateNurseUser();
            await CreateTechnicianUser();
            await CreatePatientUser();

            await _unitOfWork.CommitAsync();
        }

        private async Task CreateAdminUser()
        {
            var admin = new User(
                UserId.NewUserId(),
                new Username("admin"),
                new Email("admin@example.com"),
                new FullName("Admin User"),
                new PhoneNumber("1111111"),
                Role.admin
            );
            admin.SetPassword("12345678A@");
            await _userRepository.AddAsync(admin);
        }

        private async Task CreateDoctorUser()
        {
            var doctor = new User(
                UserId.NewUserId(),
                new Username("doctor"),
                new Email("doctor@example.com"),
                new FullName("Doctor User"),
                new PhoneNumber("2222222"),
                Role.doctor
            );
            doctor.SetPassword("12345678A@!");
            await _userRepository.AddAsync(doctor);
        }

        private async Task CreateNurseUser()
        {
            var nurse = new User(
                UserId.NewUserId(),
                new Username("nurse"),
                new Email("nurse@example.com"),
                new FullName("Nurse User"),
                new PhoneNumber("3333333"),
                Role.nurse
            );
            nurse.SetPassword("12345678A@!");
            await _userRepository.AddAsync(nurse);
        }

        private async Task CreateTechnicianUser()
        {
            var technician = new User(
                UserId.NewUserId(),
                new Username("technician"),
                new Email("technician@example.com"),
                new FullName("Technician User"),
                new PhoneNumber("4444444"),
                Role.technician
            );
            technician.SetPassword("12345678A@!");
            await _userRepository.AddAsync(technician);
        }

        private async Task CreatePatientUser()
        {
            var patientUser = new User(
                UserId.NewUserId(),
                new Username("patient"),
                new Email("patient@example.com"),
                new FullName("Patient User"),
                new PhoneNumber("5555555"),
                Role.patient
            );
            patientUser.SetPassword("12345678A@!");
            await _userRepository.AddAsync(patientUser);

            var patient = new Patient(
                patientUser,
                new EmergencyContact(
                    new PhoneNumber("6666666"),
                    new FullName("Emergency Contact"),
                    new Email("emergency@example.com")
                ),
                new List<MedicalCondition> { new MedicalCondition("None") },
                DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                Gender.male,
                new List<Appointment>()
            );
            await _patientRepository.AddAsync(patient);
        }
    }
}