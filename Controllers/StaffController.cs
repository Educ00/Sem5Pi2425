using System;
using System.Linq;
using System.Threading.Tasks;
using Sem5Pi2425.Domain.StaffAggr;

using Microsoft.AspNetCore.Mvc;
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

        public StaffController(StaffService.IStaffService staffService,UserService userService)
        {
            this._service = staffService;
            this._userService = userService;
        }
        
        [HttpPost]
        public async Task<ActionResult<StaffDTO>> CreateStaff(StaffDTO dto) {
            try {
                // Retrieve all users
                var users = await _userService.GetAllUsersAsync();

                // Print the list of users
                Console.WriteLine("Available Users:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Email: {user.Email}");
                }

                // Let the admin choose a user by ID
                Console.WriteLine("Enter the ID of the user to create staff:");
                var chosenUserId = Console.ReadLine();

                // Find the chosen user
                UserDto chosenUser = users.FirstOrDefault(u => u.Id.ToString() == chosenUserId);
                if (chosenUser == null)
                {
                    return BadRequest(new { Message = "Invalid user ID" });
                }
                var staff = await _service.CreateStaff(dto,chosenUser);
                return Ok(staff);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        [HttpPut]
        public async Task<ActionResult<StaffDTO>> EditStaff(StaffDTO dto ) {
            try {
                
                // Retrieve all users
                var users = await _userService.GetAllUsersAsync();

                // Print the list of users
                Console.WriteLine("Available Users:");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Username: {user.Username}, Email: {user.Email}");
                }

                // Let the admin choose a user by ID
                Console.WriteLine("Enter the ID of the user to create staff:");
                var chosenUserId = Console.ReadLine();

                // Find the chosen user
                UserDto chosenUser = users.FirstOrDefault(u => u.Id.ToString() == chosenUserId);
                if (chosenUser == null)
                {
                    return BadRequest(new { Message = "Invalid user ID" });
                }
                
                Console.WriteLine("Enter the number of the new specialization:");
                var chosenSpecializationNumber = Console.ReadLine();

                // Parse the chosen specialization
                if (!Enum.TryParse<Specialization>(chosenSpecializationNumber, out var chosenSpecialization))
                {
                    return BadRequest(new { Message = "Invalid specialization number" });
                }
                
                
                var staff = await _service.EditStaff(dto,chosenUser,chosenSpecialization);
                return Ok(staff);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }
        
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