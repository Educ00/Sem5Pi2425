using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Domain.OperationTypeAggr;

public class OperationTypeService : OperationTypeService.IOperationTypeService
{
    private readonly IOperationTypeRepository _operationTypeRepository;
    private readonly UserService _userService;
    private readonly IUnitOfWork _unitOfWork; // Assuming you'll need this

    public OperationTypeService(IOperationTypeRepository operationTypeRepository , UserService userService, IUnitOfWork unitOfWork)
    {
        _operationTypeRepository = operationTypeRepository;
        _userService = userService; // Initialize userService here
        _unitOfWork = unitOfWork; // Initialize unitOfWork here
    }
    
    public interface IOperationTypeService
    {
        Task<ActionResult<OperationTypeDto>> CreateOperationType(OperationTypeDto dto);
        Task<ActionResult<OperationTypeDto>> EditOperationType(OperationTypeDto dto);
    }

    public async Task<ActionResult<OperationTypeDto>> CreateOperationType(OperationTypeDto dto)
    {
        
        var operationType = new OperationType(
            dto.Duration,
            dto.Name,
            dto.Description,
            new List<Staff>()
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
        
        await _operationTypeRepository.AddAsync(operationType);
        await _unitOfWork.CommitAsync();
        
        return new OperationTypeDto(operationType);
    }
}