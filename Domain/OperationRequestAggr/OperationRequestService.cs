using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.AppointmentAggr;
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
        private readonly StaffService _staffService;
        private readonly UserService _userService;
        private readonly IAppointmentRepository _appointmentRepo;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo, StaffService staffService, 
            UserService userService, IAppointmentRepository appointmentRepo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _staffService = staffService;
            _userService = userService;
            _appointmentRepo = appointmentRepo;
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
        
        public async Task<ActionResult<OperationRequestDTO>> AddOperationRequestAsync(OperationRequestDTO dto, StaffDTO doctorStaff) {
            var operationRequest = new OperationRequest(dto.Deadline, dto.Priority, dto.Doctor, dto.Patient, dto.OperationType);
            
            if (operationRequest.OperationType.NeededSpecializations.Contains(doctorStaff.Specialization.ToString()))
            {
                await _repo.AddAsync(operationRequest);
                await _unitOfWork.CommitAsync();
                return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, 
                    operationRequest.Doctor, operationRequest.Patient, operationRequest.OperationType);
            }
            return null;
        }
        
        public async Task<ActionResult<OperationRequestDTO>> DeleteOperationRequestAsync(OperationRequestId operationRequestId) {
            Console.WriteLine("OPERATIONREQUESTID SERVICE->" + operationRequestId.Value);
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