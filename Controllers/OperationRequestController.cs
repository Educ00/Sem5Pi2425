using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.OperationRequestAggr;

namespace Sem5Pi2425.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationRequestController : ControllerBase
    {
        private readonly OperationRequestService _service;
        
        
        public OperationRequestController(OperationRequestService service) {
            _service = service;
        }
        
        // GET: api/OperationRequest
        [Authorize(Roles = "admin,doctor")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OperationRequestDTO>>> GetAll() {
            return await _service.GetAllOperationRequestsAsync();
        }
        
        // GET: api/OperationRequest/id
        [Authorize(Roles = "admin,doctor")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OperationRequestDTO>> GetById(string id) {
            try
            {
                var prod = await _service.GetOperationRequestByIdAsync(new OperationRequestId(id));

                if (prod == null) {
                    return NotFound();
                }

                return prod;
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // POST: api/OperationRequest/create-request
        [Authorize(Roles = "admin,doctor")]
        [HttpPost("create-request")]
        public async Task<ActionResult<OperationRequestDTO>> CreateOperationRequest([FromBody] CreateOperationRequestDto dto) {
            try {
                var loggedUser = HttpContext.User;
                var email = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
               var operationRequest = await _service.AddOperationRequestAsync(dto, email);
                
               return Ok(operationRequest);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // DELETE: api/OperationRequest/id
        [Authorize(Roles = "admin,doctor")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationRequestDTO>> RemoveOperationRequest(string id) {
            try
            {
                var loggedUser = HttpContext.User;
                var email = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var operationRequest = await _service.DeleteOperationRequestAsync(new OperationRequestId(id), email);
                if (operationRequest == null) {
                    return NotFound();
                }
                 return Ok(operationRequest);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // POST: api/OperationRequest/update/id
        [Authorize(Roles = "admin,doctor")]
        [HttpPatch("update/{id}")]
        public async Task<ActionResult<OperationRequestDTO>> UpdateOperationRequest(string id, [FromBody] CreateOperationRequestDto dto) {
            try {
                var loggedUser = HttpContext.User;
                var email = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;


                //var id = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var patient = await _service.UpdateOperationRequestAsync(new OperationRequestId(id), dto, email);

                return Ok(patient);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
    }
}