using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Controllers;
[ApiController]
[Route("api/[controller]")]
public class OperationTypeController : ControllerBase
{
    private readonly OperationTypeService _service;
    private readonly UserService _userService;

    public OperationTypeController(OperationTypeService operationTypeRepository,UserService userService)
    {
        this._service = operationTypeRepository;
        this._userService = userService;
    }
    
    [HttpPost]
    public async Task<ActionResult<OperationTypeDto>> AddOperationType(OperationTypeDto dto) {
        
        try {
            var operationType = await _service.CreateOperationType(dto);
            return Ok(operationType);
        }
        catch (BusinessRuleValidationException e) {
            return BadRequest(new { Message = e.Message });
        }

    }
    
    [HttpPut]
    public async Task<ActionResult<OperationTypeDto>> EditOperationType(OperationTypeDto dto) {
        
        try {
            var operationType = await _service.EditOperationType(dto);
            return Ok(operationType);
        }
        catch (BusinessRuleValidationException e) {
            return BadRequest(new { Message = e.Message });
        }

    }
    
    // Inactivate: api/TypeOfOperation/inactivate/id
    [HttpPatch("inactivate/{id}")]
    public async Task<ActionResult<OperationTypeDto>> Inactivate(string id) {
        try {
            var user = await _service.InactivateOperationType(id);

            if (user == null) {
                return NotFound();
            }

            return Ok(user);
        }
        catch (BusinessRuleValidationException e) {
            return BadRequest(new { Message = e.Message });
        }
    }
}