using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Sem5Pi2425.Domain.StaffAggr;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sem5Pi2425.Domain.LogAggr;
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
        private readonly LogService _logger;
        public StaffController(StaffService.IStaffService staffService,UserService userService,LogService logger)
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
                
            
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));
                    
                    return BadRequest(new { Message = errors });
                }

                var result = await _service.CreateStaff(dto);
                return Ok(result);
            }
            catch (BusinessRuleValidationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { Message = "An unexpected error occurred while creating the staff member." });
            }
        }
       // [Authorize(Roles = "admin")]
       [HttpPut("EditStaff/{email}")]
       public async Task<ActionResult<StaffDTO>> EditStaff(string email, [FromBody] StaffEditDto dto)
       {
           try
           {
               // Log incoming data
               Console.WriteLine($"Email from route: {email}");
               Console.WriteLine($"DTO content: {System.Text.Json.JsonSerializer.Serialize(dto)}");

               // First check if user exists
               var user = await _userService.GetUserByEmailAsync(email);
               if (user == null)
               {
                   return NotFound(new { Message = $"User with email {email} not found" });
               }

               Console.WriteLine($"User found: {user.Email}");

               // Validate the DTO
               if (!ModelState.IsValid)
               {
                   var errors = string.Join("; ", ModelState.Values
                       .SelectMany(x => x.Errors)
                       .Select(x => x.ErrorMessage));
                   Console.WriteLine($"Model validation failed: {errors}");
                   return BadRequest(new { Message = errors });
               }

               var staff = await _service.EditStaff(user, dto);
               return Ok(staff);
           }
           catch (BusinessRuleValidationException e)
           {
               Console.WriteLine($"Business rule validation failed: {e.Message}");
               return BadRequest(new { Message = e.Message });
           }
           catch (Exception ex)
           {
               Console.WriteLine($"Unexpected error: {ex.Message}");
               Console.WriteLine($"Stack trace: {ex.StackTrace}");
               return StatusCode(500, new { Message = "An unexpected error occurred" });
           }
       }
        [Authorize(Roles = "admin")]
        [HttpPut("Inactivate{id}")]
        public async Task<ActionResult<StaffDTO>> Inactivate(string id) {
            try {
                var user = await _userService.GetUserByEmailAsync(id);;

                if (user == null) {
                    return NotFound();
                }
                var staff= await _service.InactivateStaff(user);
                    _logger.AddLogAsync(new LogDto(Guid.NewGuid().ToString(), "Staff", "Staff member inactivated", user.Id.ToString(), DateTime.Now.ToString()));
                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

      //  [Authorize(Roles = "admin")]
        [HttpGet("ListStaff")]

public async Task<ActionResult<IEnumerable<StaffDTO>>> ListStaff([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string filter = null)
{
    try
    {
        var staffList = await _service.ListStaff();

        if (!string.IsNullOrEmpty(filter))
        {
            staffList = staffList.Where(s => s.User.FullName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                             s.User.Email.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                                             s.Specialization.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalRecords = staffList.Count;
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        staffList = staffList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        if (!staffList.Any())
        {
            return NotFound(new { Message = "No staff members found." });
        }

        // Print the list of staff to the console
        foreach (var staff in staffList)
        {
            Console.WriteLine($"ID: {staff.UniqueIdentifier}, Name: {staff.User.FullName}, Email: {staff.User.Email}");
        }

        // Prompt user for action
        Console.WriteLine("Enter the email address of the staff member you want to edit or deactivate:");
        var staffId = Console.ReadLine();

        Console.WriteLine("Enter 'edit' to edit the staff member or 'deactivate' to deactivate the staff member:");
        var action = Console.ReadLine();

        if (action.Equals("edit", StringComparison.OrdinalIgnoreCase))
        {
            // Call EditStaff method
            Console.WriteLine("Enter the new details for the staff member:");
            var user = await _userService.GetUserByEmailAsync(staffId);
            StaffEditDto staff = await _service.GetStaffByEmail(staffId);
            var result = await _service.EditStaff(user, staff);
            return Ok(result);
        }
        else if (action.Equals("deactivate", StringComparison.OrdinalIgnoreCase))
        {
            // Call InactivateStaff method
            var user = await _userService.GetUserByEmailAsync(staffId);
            var result = await _service.InactivateStaff(user);
            return Ok(result);
        }
        else
        {
            return BadRequest(new { Message = "Invalid action specified." });
        }
    }
    catch (BusinessRuleValidationException e)
    {
        return BadRequest(new { Message = e.Message });
    }
    catch (Exception) 
    {
        return BadRequest(new { Message = "An unexpected error occurred " });
    }
}
    
    
    
    }
}