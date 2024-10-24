using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Domain.OperationTypeAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationTypeController : ControllerBase {
        private readonly OperationTypeService _service;
        private readonly UserService _userService;
        private readonly LogService _logService;


        public OperationTypeController(OperationTypeService operationTypeRepository, UserService userService, LogService logService)
        {
            this._service = operationTypeRepository;
            this._userService = userService;
            _logService = logService;
        }

        // GET: api/OperationType
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationTypeDto>>> GetAll()
        {
            return await _service.GetAllOperationTypesAsync();
        }

        [HttpPost("add")]
        public async Task<ActionResult<OperationTypeDto>> AddOperationType(CreateOperationTypeDto dto)
        {
            try
            {
                var operationType = await _service.CreateOperationType(dto);
    
                return Ok(operationType);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }

        }
       // [Authorize(Roles = "admin")]
       [HttpPut("Edit")]
       public async Task<ActionResult<OperationTypeDto>> EditOperationType(EditOperationTypeDto dto)
       {
           try
           {
               // Retrieve all operation types
               var operationTypes = await _service.GetAllOperationTypesAsync();

               // Print the list of operation types to the console
               foreach (var operationTypeDto in operationTypes)
               {
                   Console.WriteLine($"ID: {operationTypeDto.Id.Value}, Name: {operationTypeDto.Name.Value}, Description: {operationTypeDto.Description.Value}");
               }

               // Prompt user to choose an operation type by ID
               Console.WriteLine("Enter the ID of the operation type you want to choose:");
               var chosenId = Console.ReadLine();

               // Search for the operation type by ID
               var operationType = await _service.GetOperationTypeByIdAsync(chosenId);
               if (operationType == null)
               {
                   return NotFound(new { Message = "Operation type not found." });
               }

               // Proceed with the edit operation
               var updatedOperationType = await _service.EditOperationType(operationType, dto);
               return Ok(updatedOperationType);
           }
           catch (BusinessRuleValidationException e)
           {
               return BadRequest(new { Message = e.Message });
           }
           catch (Exception) 
           {
               return BadRequest(new { Message = "An unexpected error occurred while editing the operation type." });
           }
       }
        // Inactivate: api/OperationType/inactivate/id
        [Authorize(Roles = "admin")]
        [HttpPatch("inactivate/{id}")]
        public async Task<ActionResult<OperationTypeDto>> Inactivate(string id) {
            try {
                var operationType = await _service.InactivateOperationType(id);
                if (operationType == null) {
                    return NotFound();
                }
                return Ok(operationType);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // GET: api/OperationType/search
        [Authorize(Roles = "admin")]
        [HttpGet("search")]
        public async Task<ActionResult<List<OperationTypeDto>>> SearchOperationTypes([FromQuery] string name, [FromQuery] string specialization, [FromQuery] bool? status) {
            var results = await _service.SearchOperationTypes(name, specialization, status);
            return Ok(results);
        }
    }
}