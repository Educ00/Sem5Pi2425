using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SurgeryRoomAggr;
using Status = Sem5Pi2425.Domain.AppointmentAggr.Status;

namespace Sem5Pi2425.Infrastructure.BootstrapInfra {
    public class UserBootstrapService {
        private readonly IUserRepository _userRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IOperationRequestRepository _operationRequestRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IOperationTypeRepository _operationTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserBootstrapService> _logger;

        public UserBootstrapService(IUserRepository userRepository, IPatientRepository patientRepository,
            IOperationRequestRepository operationRequestRepository, IStaffRepository staffRepository,
            IAppointmentRepository appointmentRepository, IOperationTypeRepository operationTypeRepository, IUnitOfWork unitOfWork, ILogger<UserBootstrapService> logger) {
            _userRepository = userRepository;
            _patientRepository = patientRepository;
            _operationRequestRepository = operationRequestRepository;
            _staffRepository = staffRepository;
            _appointmentRepository = appointmentRepository;
            _operationTypeRepository = operationTypeRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task BootstrapInitialUsers() {
            _logger.LogInformation("Starting to bootstrap initial users");
            try {
                await CreateAdminUser();
                await CreateDoctorUser();
                await CreateNurseUser();
                await CreateTechnicianUser();
                await CreatePatientUser();

                await CreateOperationRequest();
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Successfully bootstrapped initial users");
            }
            catch (Exception e) {
                _logger.LogError(e, "An error occurred while bootstrapping initial users");
                throw;
            }
        }

        private async Task CreateAdminUser() {
            _logger.LogInformation("Creating admin user");
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
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Admin user created successfully");
        }

        private async Task CreateDoctorUser() {
            _logger.LogInformation("Creating doctor user");
            var doctor = new User(
                UserId.NewUserId(),
                new Username("doctor"),
                new Email("doctor@example.com"),
                new FullName("Doctor User"),
                new PhoneNumber("2222222"),
                Role.doctor
            );
            doctor.SetPassword("12345678A@");
            await _userRepository.AddAsync(doctor);

            var staffDoctor = new Staff(
                doctor,
                new List<AvailableSlots>(),
                UniqueIdentifier.CreateFromString(doctor.Id.ToString()),
                Specialization.orthopedics
            );
            await _staffRepository.AddAsync(staffDoctor);

            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Doctor user created successfully");
        }

        private async Task CreateNurseUser() {
            _logger.LogInformation("Creating nurse user");
            var nurse = new User(
                UserId.NewUserId(),
                new Username("nurse"),
                new Email("nurse@example.com"),
                new FullName("Nurse User"),
                new PhoneNumber("3333333"),
                Role.nurse
            );
            nurse.SetPassword("12345678A@");
            await _userRepository.AddAsync(nurse);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Nurse user created successfully");
        }

        private async Task CreateTechnicianUser() {
            _logger.LogInformation("Creating technician user");
            var technician = new User(
                UserId.NewUserId(),
                new Username("technician"),
                new Email("technician@example.com"),
                new FullName("Technician User"),
                new PhoneNumber("4444444"),
                Role.technician
            );
            technician.SetPassword("12345678A@");
            await _userRepository.AddAsync(technician);
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Technician user created successfully");
        }

        private async Task CreatePatientUser() {
            _logger.LogInformation("Creating patient user");
            var patientUser = new User(
                UserId.NewUserId(),
                new Username("patient"),
                new Email("patient@example.com"),
                new FullName("Patient User"),
                new PhoneNumber("5555555"),
                Role.patient
            );
            patientUser.SetPassword("12345678A@");
            await _unitOfWork.CommitAsync();
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
            await _unitOfWork.CommitAsync();
            _logger.LogInformation("Patient user created successfully");
        }

        private async Task CreateOperationRequest() {
            _logger.LogInformation("Creating operation request");
            User patientUser = await _userRepository.GetByEmailAsync("patient@example.com");
            Patient patient = await _patientRepository.GetByIdAsync(patientUser.Id);
            User doctor = await _userRepository.GetByUsername("doctor");
            Staff doctorStaff = await _staffRepository.GetByIdAsync(doctor.Id);

            List<Staff> listStaff = new List<Staff>();
            listStaff.Add(doctorStaff);
            OperationType operationType = new OperationType(new DateTime(2024, 11, 1), new Name("foot operation"),
                new Description("description"), listStaff, true);
            Console.WriteLine("OPERATION TYPE ID: " + operationType.Id.Value);
            var operationRequest = new OperationRequest(
                new DateOnly(2024,11,25),
                Priority.Urgent, doctor, patient, operationType);

            await _operationRequestRepository.AddAsync(operationRequest);
            await _operationTypeRepository.AddAsync(operationType);
            await _unitOfWork.CommitAsync();
            await CreateAppointment(operationRequest);

        }

        private async Task CreateAppointment(OperationRequest operationRequest)
        {
            _logger.LogInformation("Creating appointment");
            RoomNumber roomNumber = new RoomNumber(10);
            Appointment appointment = new Appointment(Status.scheduled, new DateTime(2024, 11, 25), roomNumber,
                new List<Staff>(), operationRequest);

            Console.WriteLine(appointment.Id.Value);
            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.CommitAsync();

        }
    }
}