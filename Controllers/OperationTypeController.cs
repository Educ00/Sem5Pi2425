using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public OperationTypeController(OperationTypeService operationTypeRepository, UserService userService)
        {
            this._service = operationTypeRepository;
            this._userService = userService;
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

        [HttpPut("edit")]
        public async Task<ActionResult<OperationTypeDto>> EditOperationType(EditOperationTypeDto dto)
        {
            string id = null;
            try
            {
                var operationType = await _service.EditOperationType(id,dto);
                return Ok(operationType);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
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