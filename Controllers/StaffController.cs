using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sem5Pi2425.Domain.StaffAggr;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly StaffService.IStaffService _service;
        private readonly UserService _userService;
        private readonly ILogger<StaffController> _logger;
        public StaffController(StaffService.IStaffService staffService,UserService userService,ILogger<StaffController> logger)
        {
            this._service = staffService;
            this._userService = userService;
            _logger = logger;
        }
        [Authorize(Roles = "admin")]
        [HttpPost("CreateStaff")]
        public async Task<ActionResult<StaffDTO>> CreateStaff([FromBody] StaffCreateDTO dto) {
            try 
            {
                _logger.LogInformation($"Attempting to create staff with Email: {dto.Email}");
            
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));
                    _logger.LogWarning($"Model validation failed: {errors}");
                    return BadRequest(new { Message = errors });
                }

                var result = await _service.CreateStaff(dto);
                return Ok(result);
            }
            catch (BusinessRuleValidationException ex)
            {
                _logger.LogWarning($"Business rule validation failed: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected error creating staff: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { Message = "An unexpected error occurred while creating the staff member." });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("EditStaff/{id}")]
        public async Task<ActionResult<StaffDTO>> EditStaff(string id,[FromBody] StaffEditDto dto ) {
            try {
                
                // Retrieve all users
                var users = await _userService.GetUserByEmailAsync(id);
                
                // Find the chosen use
                

                
                
                var staff = await _service.EditStaff(users,dto);
                return Ok(staff);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        [Authorize(Roles = "admin")]
        [HttpPut("Inactivate")]
        public async Task<ActionResult<StaffDTO>> Inactivate(string id) {
            try {
                var user = await _userService.InactivateUserAsync(id);

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
    
    
    
    
}