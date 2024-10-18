using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationRequestAggr
{

    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _repo;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
        }

        
        public async Task<List<OperationRequestDTO>> GetAllOperationRequestsAsync() {
            var list = await _repo.GetAllAsync();

            List<OperationRequestDTO> listDto = list.ConvertAll(operationRequest =>
                new OperationRequestDTO(
                    operationRequest.Id,
                    operationRequest.Deadline,
                    operationRequest.Priority,
                    operationRequest.Doctor,
                    operationRequest.Patient));
            // Para testar se funfa só tirar o comentário. Deve aparecer.
           // listDTO.Add(new OperationRequestDTO(new Guid(), true, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin));
            return listDto;
        }

        public async Task<ActionResult<OperationRequestDTO>> GetOperationRequestByIdAsync(OperationRequestId id) {
            var operationRequest = await _repo.GetByIdAsync(id);

            return operationRequest == null ? null : new OperationRequestDTO(operationRequest);
        }
        
        public async Task<ActionResult<OperationRequestDTO>> AddOperationRequestAsync(OperationRequestDTO dto) {
            var operationRequest = new OperationRequest(dto.Deadline, dto.Priority, dto.Doctor, dto.Patient);
            
            var userId = UserId.NewUserId();
            var user = new User(userId, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.admin);
            var emergencyContact = new EmergencyContact(new PhoneNumber("969999999"), new FullName("Maria Da Costa"),
                new Email("maria@gmail.com"));
            var patient = new Patient(user, emergencyContact, new List<MedicalCondition>(), new DateOnly(1985,5,10), Gender.Female, MedicalRecordsNumber.NewMedicalRecordsNumber(), new List<Appointment>());
            operationRequest = new OperationRequest(new DateTime(2024, 11, 15), Priority.Urgent, user, patient);
            
            await _repo.AddAsync(operationRequest);
            await _unitOfWork.CommitAsync();
            return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, 
                operationRequest.Doctor, operationRequest.Patient);
        }
        
        public async Task<ActionResult<OperationRequestDTO>> DeleteOperationRequestAsync(OperationRequestId operationRequestId) {
            Console.WriteLine("OPERATIONREQUESTID SERVICE->" + operationRequestId.Value);
            var operationRequest = await _repo.GetByIdAsync(operationRequestId);
            
            _repo.Remove(operationRequest);
            await _unitOfWork.CommitAsync();

            return new OperationRequestDTO(operationRequest.Id, operationRequest.Deadline, operationRequest.Priority, 
                operationRequest.Doctor, operationRequest.Patient);
        }

    }
    
}