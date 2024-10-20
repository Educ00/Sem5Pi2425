using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;
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
        
        // POST: api/OperationRequest
        [Authorize(Roles = "admin,doctor")]
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateOperationRequest(OperationRequestDTO dto) {
            try {
                var operationRequest = await _service.AddOperationRequestAsync(dto);
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
                // Console.WriteLine("OPERATIONREQUESTID Controller->" + id);
                

                var operationRequest = await _service.DeleteOperationRequestAsync(new OperationRequestId(id));
                if (operationRequest == null) {
                    return NotFound();
                }

                 return Ok(operationRequest);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }

        }

    }
}