using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.OperationRequestAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class OperationTypeService : OperationTypeService.IOperationTypeService
{
    private readonly IOperationTypeRepository _operationTypeRepository;
    private readonly IUnitOfWork _unitOfWork; 

    public OperationTypeService(IOperationTypeRepository operationTypeRepository, IUnitOfWork unitOfWork)
    {
        _operationTypeRepository = operationTypeRepository;
        _unitOfWork = unitOfWork; 
    }
    
    public interface IOperationTypeService
    {
        Task<ActionResult<OperationTypeDto>> CreateOperationType(OperationTypeDto dto);
        Task<ActionResult<OperationTypeDto>> EditOperationType(OperationTypeDto dto);
    }
    
    public async Task<List<OperationTypeDto>> GetAllOperationTypesAsync() {
        var list = await _operationTypeRepository.GetAllAsync();
        List<OperationTypeDto> listDto = list.ConvertAll(operationRequest =>
            new OperationTypeDto(
                operationRequest.Id,
                operationRequest.Duration,
                operationRequest.Name,
                operationRequest.Description,
                operationRequest.NeededSpecializations,
                operationRequest.Active));
        return listDto;
    }

    public async Task<ActionResult<OperationTypeDto>> CreateOperationType(OperationTypeDto dto)
    {
        
        var operationType = new OperationType(
            dto.Duration,
            dto.Name,
            dto.Description,
            new List<Staff>(),
            dto.Active
        );
        
        await _operationTypeRepository.AddAsync(operationType);
        await _unitOfWork.CommitAsync();
        
        return new OperationTypeDto(operationType);
    }
    
    public async Task<ActionResult<OperationTypeDto>> EditOperationType(OperationTypeDto dto)
    {
        var operationType = await _operationTypeRepository.GetByIdAsync(dto.Id);
        operationType.Duration = dto.Duration;
        operationType.Name = dto.Name;
        operationType.Description = dto.Description;
        operationType.NeededSpecializations = dto.NeededSpecializations;
        operationType.Active = dto.Active;
        
        await _operationTypeRepository.AddAsync(operationType);
        await _unitOfWork.CommitAsync();
        
        return new OperationTypeDto(operationType);
    }
    
    public async Task<ActionResult<OperationTypeDto>> InactivateOperationType(string operationTypeId)
    {
        var operationType = await _operationTypeRepository.GetByIdAsync(new OperationTypeId(operationTypeId));
        
        if (operationType == null) {
            return null;
        }
        
        operationType.MarkAsInative();
        await _unitOfWork.CommitAsync();

        return new OperationTypeDto(operationType.Id, operationType.Duration, 
            operationType.Name, operationType.Description, operationType.NeededSpecializations, operationType.Active);
    }
    
    public async Task<List<OperationTypeDto>> SearchOperationTypes(string name, string specialization, bool? status) {
        var operationTypes = await _operationTypeRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(name)) {
            operationTypes = operationTypes.Where(ot => ot.Name.Value.Contains(name)).ToList();
        }

        if (!string.IsNullOrEmpty(specialization)) {
            operationTypes = operationTypes.Where(ot => ot.NeededSpecializations.Contains(specialization)).ToList();
        }

        if (status.HasValue) {
            operationTypes = operationTypes.Where(ot => ot.Active == status.Value).ToList();
        }

        return operationTypes.Select(ot => new OperationTypeDto(ot)).ToList();
    }
}