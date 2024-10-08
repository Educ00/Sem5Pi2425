﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUser;

namespace Sem5Pi2425.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UserService _service;
        private readonly IWebHostEnvironment _environment;

        public UsersController(UserService service, IWebHostEnvironment environment) {
            this._service = service;
            this._environment = environment;
        }
        
        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() {
            return await this._service.GetAllUsersAsync();
        }
        
        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id) {
            try
            {
                var prod = await this._service.GetUserByIdAsync(new UserId(id));

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
        public async Task<ActionResult<UserDto>> CreateUser(UserDto dto) {
            try {
                var user = await _service.AddUserAsync(dto);
                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // Post: api/Users/backoffice
        [HttpPost("backoffice")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<UserDto>> CreateBackofficeUser(CreateBackofficeUserDto dto) {
            try {
                var user = await _service.CreateBackofficeUserAsync(dto);
                return Ok(user);
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
                var user = await _service.InactivateUserAsync(userId);

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
        
        // GET: api/Users/activate
        [HttpGet("activate")]
        [AllowAnonymous]
        public IActionResult ActivateUserPage([FromQuery] string token)
        {
            var path = Path.Combine(_environment.WebRootPath, "activate.html");
            return PhysicalFile(path, "text/html");
        }
        
        // Activate: api/Users
        [HttpPost("activate")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> ActivateUser([FromBody] ActivateUserDto dto)
        {
            try
            {
                var user = await _service.ActivateUserAsync(dto);
                return Ok(user);
            }
            catch (BusinessRuleValidationException e)
            {
                return BadRequest(new { Message = e.Message });
            }
        }
        
        // DELETE: api/Users/5/delete
        [HttpDelete("{id}/delete")]
        public async Task<ActionResult<UserDto>> HardDelete(UserId userId) {
            try
            {
                var user = await _service.DeleteUserAsync(userId);

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