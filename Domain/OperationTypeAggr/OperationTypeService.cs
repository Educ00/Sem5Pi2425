using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

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
        Task<ActionResult<OperationTypeDto>> CreateOperationType(CreateOperationTypeDto dto);
        Task<ActionResult<OperationTypeDto>> EditOperationType(EditOperationTypeDto operationTypeDto,EditOperationTypeDto dto);
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

    public async Task<ActionResult<OperationTypeDto>> CreateOperationType(CreateOperationTypeDto dto)
    {
        List<OperationType> operationTypes = await _operationTypeRepository.GetAllAsync();
        if (operationTypes.Any(ot => ot.Name.Value == dto.Name))
        {
            throw new BusinessRuleValidationException("Operation type with this name already exists");
        }
        var duration = Convert.ToDateTime(dto.Duration);
        var name = new Name(dto.Name);
        var description = new Description(dto.Description);
        var neededSpecializations = new List<Staff>();
        var active=true;
        var operationType = new OperationType( duration, name, description, neededSpecializations, active);
        
        await _operationTypeRepository.AddAsync(operationType);
        await _unitOfWork.CommitAsync();
        
        return new OperationTypeDto(operationType);
    }



    public async Task<ActionResult<OperationTypeDto>> EditOperationType(EditOperationTypeDto operationTypeDto, EditOperationTypeDto dto)
    {
        try
        {
            var operationType = await _operationTypeRepository.GetByIdAsync(new OperationTypeId(operationTypeDto.Id));
            if (operationType == null)
            {
                throw new BusinessRuleValidationException($"OperationType with ID {operationType.Id} not found");
            }

            DateTime? newDuration = null;
            Name newName = null;
            Description newDescription = null;
            List<string> newNeededSpecialization = null;

            if (!string.IsNullOrEmpty(dto.Name))
            {
                try
                {
                    newName = new Name(dto.Name.Trim());
                }
                catch (ArgumentException ex)
                {
                    throw new BusinessRuleValidationException($"Invalid name: {ex.Message}");
                }
            }

            if (!string.IsNullOrEmpty(dto.Description))
            {
                try
                {
                    newDescription = new Description(dto.Description.Trim());
                }
                catch (ArgumentException ex)
                {
                    throw new BusinessRuleValidationException($"Invalid description: {ex.Message}");
                }
            }

            if (newDuration != null) {
                 operationType.UpdateDuration(newDuration.Value);
            }
            
            if (newName != null)
            {
                operationType.UpdateName(newName);
            }

            if (newDescription != null)
            {
                operationType.UpdateDescription(newDescription);
            }

            if (newNeededSpecialization != null)
            {
                operationType.UpdateNeededSpecialization(newNeededSpecialization);
            }

            await _unitOfWork.CommitAsync();
            return new OperationTypeDto(operationType.Id, operationType.Duration, operationType.Name,
                operationType.Description, operationType.NeededSpecializations, operationType.Active);
        }
        catch (BusinessRuleValidationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BusinessRuleValidationException($"Error updating type of operation: {ex.Message}");
        }
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
    
    public async Task<EditOperationTypeDto> GetOperationTypeByIdAsync(string id)
    {
        var operationType = await _operationTypeRepository.GetByIdAsync(id);
        if (operationType == null)
        {
            throw new BusinessRuleValidationException($"Operation type with ID {id} not found");
        }

        return new EditOperationTypeDto(operationType.Id.Value, operationType.Duration.ToLongDateString(), operationType.Name.Value,operationType.Description.Value, operationType.NeededSpecializations.ToString(), operationType.Active.ToString());
    }
}