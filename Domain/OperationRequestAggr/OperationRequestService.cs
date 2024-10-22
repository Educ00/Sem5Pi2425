using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationRequestAggr
{

    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _repo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IOperationTypeRepository _operationTypeRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IUserRepository _userRepo;
        private readonly IStaffRepository _staffRepo;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo, IAppointmentRepository appointmentRepo, 
            IOperationTypeRepository operationTypeRepo, IPatientRepository patientRepo, IUserRepository userRepo, IStaffRepository staffRepository)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _appointmentRepo = appointmentRepo;
            _operationTypeRepo = operationTypeRepo;
            _patientRepo = patientRepo;
            _userRepo = userRepo;
            _staffRepo = staffRepository;
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
            var deadlineString = dto.Deadline;
            DateOnly deadline = DateOnly.Parse(deadlineString);

            Enum.TryParse(dto.Priority, true, out Priority priority);
            
            var doctor = await _userRepo.GetByEmailAsync(email);
            var doctorStaff = await _staffRepo.GetByIdAsync(doctor.Id);
            var patientUser = await _userRepo.GetByEmailAsync(dto.Patient.Email);
            var patient = await _patientRepo.GetByIdAsync(patientUser.Id);
            var operationType = await _operationTypeRepo.GetByIdAsync(dto.OperationType.Id);
            
            if (doctor == null) {
                throw new Exception("Doctor not found");
            }
            if (patient == null) {
                throw new Exception("Patient not found");
            }
            if (operationType == null) {
                throw new Exception("Operation type not found");
            }
            
            if (operationType.NeededSpecializations.Contains(doctorStaff.Specialization.ToString())) {
                Console.WriteLine("OPERATION MATCHES");
                var operationRequest = new OperationRequest(deadline,priority, doctor, patient, operationType);

                await _repo.AddAsync(operationRequest);
                await _unitOfWork.CommitAsync();

                return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority,
                    operationRequest.Doctor, operationRequest.Patient, operationRequest.OperationType);
            }
            throw new Exception("Operation type doesn't match the doctor’s specialization.");
        }
        
        public async Task<ActionResult<OperationRequestDTO>> DeleteOperationRequestAsync(OperationRequestId operationRequestId) {
            var operationRequest = await _repo.GetByIdAsync(operationRequestId);
            var appointments = await _appointmentRepo.GetAllAsync();

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

    }
}