using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUser;

namespace Sem5Pi2425.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UserService _service;
        
        public UsersController(UserService service) {
            this._service = service;
        }
        
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() {
            return await this._service.GetAllAsync();
        }
        
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(UserId id) {
            try
            {
                var prod = await this._service.GetByIdAssync(id);

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
        
        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserDto dto) {
            try {
                var user = await _service.AddAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = user.Value.Id }, user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // Inactivate: api/Users/5/inactivate
        [HttpPatch("{id}/inactivate")]
        public async Task<ActionResult<UserDto>> Inactivate(UserId userId) {
            try
            {
                var user = await _service.InactivateAsync(userId);

                if (user == null) {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // Activate: api/Users/5/activate
        [HttpPatch("{id}/activate")]
        public async Task<ActionResult<UserDto>> Activate(UserId userId) {
            try {
                var user = await _service.ActivateAsync(userId);

                if (user == null) {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest( new {Message = e.Message});
            }
        }
        
        // DELETE: api/Users/5/delete
        [HttpDelete("{id}/delete")]
        public async Task<ActionResult<UserDto>> HardDelete(UserId userId) {
            try
            {
                var user = await _service.DeleteAsync(userId);

                if (user == null) {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        
        
        // FALTAM OUTROS METODOS
    }
}