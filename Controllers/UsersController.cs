using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UserService _userUserService;
        private readonly PatientService _patientService;

        public UsersController(UserService userService, PatientService patientService) {
            this._userUserService = userService;
            this._patientService = patientService;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() {
            return await this._userUserService.GetAllUsersAsync();
        }

        // GET: api/Users/id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(string id) {
            try {
                var prod = await this._userUserService.GetUserByIdAsync(new UserId(id));

                if (prod == null) {
                    return NotFound();
                }

                return prod;
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateUser(UserDto dto) {
            try {
                var user = await _userUserService.AddUserAsync(dto);
                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        /*
        [HttpPost("patient")]
        public async Task<ActionResult<PatientDto>> CreatePatient(UserDto userDto, PatientDto patientDto) {
            try {
                var patient = await _userUserService.AddPatientAsync(userDto, patientDto);
                return Ok(patient);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { e.Message });
            }
        }
        */


        // DELETE: api/Users/id
            [HttpDelete("{id}")]
            public async Task<ActionResult<UserDto>> HardDelete(string id) {
                try {
                    Console.WriteLine("USERID Controller->" + id);

                    var user = await _userUserService.DeleteUserAsync(new UserId(id));
                    Console.WriteLine("Encontrei este nino->" + user.Value.FullName);
                    if (user == null) {
                        return NotFound();
                    }

                    return Ok(user);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            // Post: api/Users/backoffice/create
            [HttpPost("backoffice/create")]
            //[Authorize(Roles = "admin")]
            public async Task<ActionResult<UserDto>> CreateBackofficeUser(CreateBackofficeUserDto dto) {
                try {
                    var user = await _userUserService.CreateBackofficeUserAsync(dto);
                    return Ok(user);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            // Inactivate: api/Users/inactivate/id
            [HttpPatch("inactivate/{id}")]
            public async Task<ActionResult<UserDto>> Inactivate(string id) {
                // TODO: Test if this method actually works!..
                try {
                    var user = await _userUserService.InactivateUserAsync(id);

                    if (user == null) {
                        return NotFound();
                    }

                    return Ok(user);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            /*
            // GET: api/Users/activate
            [HttpGet("activate")]
            [AllowAnonymous]
            public IActionResult ActivateUserPage([FromQuery] string token)
            {
                var path = Path.Combine(_environment.WebRootPath, "activate.html");
                return PhysicalFile(path, "text/html");
            }
            */

            // Activate: api/Users/activate
            [HttpPost("activate")]
            [AllowAnonymous]
            public async Task<ActionResult<UserDto>> ActivateUser([FromBody] UserPasswordDto passwordDto) {
                try {
                    var user = await _userUserService.ActivateUserAsync(passwordDto);
                    return Ok(user);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            // POST: api/Users/backoffice/request-password-reset
            [HttpPost("backoffice/request-password-reset")]
            [AllowAnonymous]
            public async Task<ActionResult> RequestPasswordReset([FromBody] string email) {
                try {
                    await _userUserService.RequestPasswordResetAsync(email);
                    return Ok("Password request sent to " + email);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            // POST: api/Users/backoffice/reset-password
            [HttpPost("backoffice/reset-password")]
            public async Task<ActionResult<UserDto>> ResetPassword([FromBody] UserPasswordDto userPasswordDto) {
                try {
                    var user = await _userUserService.CompletePasswordReset(userPasswordDto);
                    return Ok(user);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }

            // POST: api/Users/Patient/signin
            [HttpPost("Patient/signin")]
            public async Task<ActionResult<PatientDto>> SignIn([FromBody] RegisterPatientDto dto) {
                try {
                    var patient = await _patientService.SignIn(dto);
                    return Ok(patient);
                }
                catch (BusinessRuleValidationException e) {
                    return BadRequest(new { Message = e.Message });
                }
            }


            // FALTAM OUTROS METODOS
        }
    }