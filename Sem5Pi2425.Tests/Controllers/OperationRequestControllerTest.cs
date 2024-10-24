using Microsoft.AspNetCore.Mvc;
using Moq;
using Sem5Pi2425.Controllers;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Xunit;
using Xunit.Abstractions;


namespace Sem5Pi2425.Tests.Controllers {

    public class OperationRequestControllerTest {
        private readonly ITestOutputHelper _testOutputHelper;

        private readonly Mock<OperationRequestService.IOperationRequestService> _mockService;
        private readonly OperationRequestController _controller;

        public OperationRequestControllerTest(ITestOutputHelper testOutputHelper) {
            _testOutputHelper = testOutputHelper;
            _mockService = new Mock<OperationRequestService.IOperationRequestService>();
            _controller = new OperationRequestController(_mockService.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsAllOperationRequests()
        {
            // Arrange
            // Simule a chamada ao método que você quer testar

            var doctor = new User(
                UserId.NewUserId(),
                new Username("doctor2"),
                new Email("doctor2@example.com"),
                new FullName("Doctor2 User"),
                new PhoneNumber("22222222"),
                Role.doctor
            );
            doctor.SetPassword("12345678A@");

            var staffDoctor = new Staff(
                doctor,
                new List<AvailableSlots>(),
                UniqueIdentifier.CreateFromString(doctor.Id.ToString()),
                Specialization.orthopedics
            );

            var patientUser = new User(
                UserId.NewUserId(),
                new Username("patient"),
                new Email("patient@example.com"),
                new FullName("Patient User"),
                new PhoneNumber("5555555"),
                Role.patient
            );
            patientUser.SetPassword("12345678A@");

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

            var listStaff = new List<Staff>();
            listStaff.Add(staffDoctor);

            var operationType = new OperationType(new DateTime(2024, 11, 1), new Name("foot operation"),
                new Description("foot or ankle operation"), listStaff, true);

            var operationRequest = new OperationRequestDTO(
                new OperationRequestId("1"),
                new DateOnly(2024, 10, 25),
                Priority.Emergency,
                doctor,
                patient,
                operationType);


            var operationRequest2 = new OperationRequestDTO(
                new OperationRequestId("2"),
                new DateOnly(2024, 11, 25),
                Priority.Urgent,
                doctor,
                patient,
                operationType);

            var operationRequests = new List<OperationRequestDTO> { operationRequest };
            
            operationRequests.Add(operationRequest2);
            _mockService
                .Setup(service => service.GetAllOperationRequestsAsync())
                .ReturnsAsync(operationRequests);

            ActionResult<IEnumerable<OperationRequestDTO>> result = await _controller.GetAll();
            
            _testOutputHelper.WriteLine($"Total Operation Requests: {operationRequests.Count()}");
            foreach (var request in operationRequests) {
                _testOutputHelper.WriteLine($"Operation Request ID: {request.Id.Value}, Doctor: {request.Doctor.Username.Value}");
            }

            var okResult = Assert.IsType<ActionResult<IEnumerable<OperationRequestDTO>>>(result);
            var returnValue = Assert.IsType<ActionResult<IEnumerable<OperationRequestDTO>>>(okResult);
            var returnOperationRequests = Assert.IsAssignableFrom<IEnumerable<OperationRequestDTO>>(returnValue.Value);
            Assert.Equal(operationRequests.Count(), returnOperationRequests.Count());
        }
        
        [Fact]
        public async Task GetById_ReturnsOperationRequest_WhenOperationRequestExists() {
            // Arrange
            var operationRequestId = "1";

            var doctor = new User(
                UserId.NewUserId(),
                new Username("doctor2"),
                new Email("doctor2@example.com"),
                new FullName("Doctor2 User"),
                new PhoneNumber("22222222"),
                Role.doctor
            );
            doctor.SetPassword("12345678A@");

            var staffDoctor = new Staff(
                doctor,
                new List<AvailableSlots>(),
                UniqueIdentifier.CreateFromString(doctor.Id.ToString()),
                Specialization.orthopedics
            );

            var patientUser = new User(
                UserId.NewUserId(),
                new Username("patient"),
                new Email("patient@example.com"),
                new FullName("Patient User"),
                new PhoneNumber("5555555"),
                Role.patient
            );
            patientUser.SetPassword("12345678A@");

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

            var listStaff = new List<Staff>();
            listStaff.Add(staffDoctor);
            
            var operationType = new OperationType(new DateTime(2024, 11, 1), new Name("foot operation"),
                new Description("foot or ankle operation"), listStaff, true);

            var operationRequest = new OperationRequestDTO(
                new OperationRequestId("1"),
                new DateOnly(2024, 10, 25),
                Priority.Emergency,
                doctor,
                patient,
                operationType);
            
            var operationRequest2 = new OperationRequestDTO(
                new OperationRequestId("2"),
                new DateOnly(2024, 11, 25),
                Priority.Urgent,
                doctor,
                patient,
                operationType);

            _testOutputHelper.WriteLine($"ID: {operationRequest.Id.Value} DOCTOR: {operationRequest.Doctor.Username.Value}");

            _mockService
                .Setup(service => service.GetOperationRequestByIdAsync(It.IsAny<OperationRequestId>()))
                .ReturnsAsync(operationRequest);
            
            
            _mockService
                .Setup(service => service.GetOperationRequestByIdAsync(It.Is<OperationRequestId>(id => id.Value == operationRequestId)))
                .ReturnsAsync(operationRequest);

            var result = await _controller.GetById("2");
            
            // Assert
            var okResult = Assert.IsType<ActionResult<OperationRequestDTO>>(result);
            var returnOperationRequest = Assert.IsType<ActionResult<OperationRequestDTO>>(okResult);
            Assert.NotNull(returnOperationRequest.Value);
            Assert.Equal(operationRequest.Id, returnOperationRequest.Value.Id);

        }
    }
}

