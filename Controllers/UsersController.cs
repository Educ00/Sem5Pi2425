using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.Users;

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
            return await this._service.GetAllAssync();
        }
        
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id) {
            var prod = await this._service.GetByIdAssync(new UserId(id));

            if (prod == null) {
                return NotFound();
            }
            return prod;
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
        
        // FALTAM OUTROS METODOS
    }
}