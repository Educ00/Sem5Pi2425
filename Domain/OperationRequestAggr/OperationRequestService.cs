using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;
using Type = Sem5Pi2425.Domain.LogAggr.Type;

namespace Sem5Pi2425.Domain.OperationRequestAggr {

    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _repo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IOperationTypeRepository _operationTypeRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;
        private readonly LogService _logService;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo, IAppointmentRepository appointmentRepo, IOperationTypeRepository operationTypeRepo, 
            IPatientRepository patientRepo, IUserRepository userRepo, IStaffRepository staffRepository, LogService logService) {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _appointmentRepo = appointmentRepo;
            _operationTypeRepo = operationTypeRepo;
            _patientRepo = patientRepo;
            _userRepo = userRepo;
            _staffRepo = staffRepository;
            _logService = logService;
        }
        
        public async Task<List<OperationRequestDTO>> GetAllOperationRequestsAsync() {
            var list = await _repo.GetAllAsync();
            List<OperationRequestDTO> listDto = list.ConvertAll(operationRequest =>
                new OperationRequestDTO(
                    operationRequest.Id,
                    operationRequest.Deadline,
                    operationRequest.Priority,
                    operationRequest.Doctor,
                    operationRequest.Patient,
                    operationRequest.OperationType));
            return listDto;
        }

        public async Task<ActionResult<OperationRequestDTO>> GetOperationRequestByIdAsync(OperationRequestId id) {
            var operationRequest = await _repo.GetByIdAsync(id);

            return operationRequest == null ? null : new OperationRequestDTO(operationRequest);
        }
        
        public async Task<ActionResult<OperationRequestDTO>> AddOperationRequestAsync(CreateOperationRequestDto dto, string email)
        {
            var log = "";
            var deadlineString = dto.Deadline;
            DateOnly deadline = DateOnly.Parse(deadlineString);
            Enum.TryParse(dto.Priority, true, out Priority priority);
            
            var doctor = await _userRepo.GetByEmailAsync(email);
            var doctorStaff = await _staffRepo.GetByIdAsync(doctor.Id);
            var patientUser = await _userRepo.GetByEmailAsync(dto.Patient.Email);
            var patient = await _patientRepo.GetByIdAsync(patientUser.Id);
            var operationType = await _operationTypeRepo.GetByIdAsync(dto.OperationType.Id);
            
            if (doctor == null) {
                throw new BusinessRuleValidationException("Doctor not found");
            }
            if (patient == null) {
                throw new BusinessRuleValidationException("Patient not found");
            }
            if (operationType == null) {
                throw new BusinessRuleValidationException("Operation type not found");
            }
            
            if (operationType.NeededSpecializations.Contains(doctorStaff.Specialization.ToString()) && operationType.Active) {
                var operationRequest = new OperationRequest(deadline,priority, doctor, patient, operationType);

                    await _repo.AddAsync(operationRequest);
                    await _unitOfWork.CommitAsync();
                    log += $"NEW REQUEST ID: {operationRequest.Id.Value}; Deadline: {operationRequest.Deadline.ToString()}; Priority: {operationRequest.Priority}; " +
                           $"Doctor: {operationRequest.Doctor.Username.Value}; Patient: {operationRequest.Patient.User.Username.Value}; Operation type: {operationRequest.OperationType.Name.Value}"; 
                    _logService.AddLogAsync(new LogDto(null, Type.OperationRequest.ToString(), log, patient.Id.Value, null));
                    return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority,
                        operationRequest.Doctor, operationRequest.Patient, operationRequest.OperationType);
            } 
            
            throw new Exception("Operation type doesn't match the doctor’s specialization.");
            
        }
        
        public async Task<ActionResult<OperationRequestDTO>> DeleteOperationRequestAsync(OperationRequestId operationRequestId, string email) {
            var operationRequest = await _repo.GetByIdAsync(operationRequestId);
            var appointments = await _appointmentRepo.GetAllAsync();
            var doctor = await _userRepo.GetByEmailAsync(email);
            var operationDoctorId = operationRequest.Doctor.Id.Value;

            if (operationDoctorId.Equals(doctor.Id.Value))
            {
                foreach (var appointment in appointments)
                {
                    if (appointment.OperationRequest.Equals(operationRequest))
                    {
                        throw new Exception("Operation request already scheduled");
                    } 
                } 
                _repo.Remove(operationRequest); 
                await _unitOfWork.CommitAsync();
            
                return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, 
                    operationRequest.Doctor, operationRequest.Patient, operationRequest.OperationType);
            }
            throw new Exception("Doctors can only delete operations requests they created");
        }

        public async Task<ActionResult<OperationRequestDTO>> UpdateOperationRequestAsync(OperationRequestId id, [FromBody] CreateOperationRequestDto dto, string email) {
            var operationRequest = await _repo.GetByIdAsync(id);
            var doctor = await _userRepo.GetByEmailAsync(email);
            var operationDoctorId = operationRequest.Doctor.Id.Value;

            ArgumentNullException.ThrowIfNull(dto);

            if (operationDoctorId.Equals(doctor.Id.Value)) {
                Enum.TryParse(dto.Priority, true, out Priority priority);
                var deadline = DateOnly.Parse(dto.Deadline);
                Console.WriteLine("DEADLINE: " + deadline);
                Console.WriteLine("PRIORITY: " + priority);

                if (!operationRequest.Deadline.Equals(deadline)) {
                    operationRequest.UpdateDeadline(deadline);
                }

                if (!operationRequest.Priority.Equals(priority))
                {
                    operationRequest.UpdatePriority(priority);
                }
            
                await _unitOfWork.CommitAsync();
                return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, 
                    operationRequest.Doctor, operationRequest.Patient, operationRequest.OperationType);
            }
            throw new Exception("Doctors can only update operations requests they created");
        }
    }
}