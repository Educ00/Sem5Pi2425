using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.LogAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.SystemUserAggr;

namespace Sem5Pi2425.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase {
        private readonly UserService _userUserService;
        private readonly PatientService _patientService;
        private readonly LogService _logService;

        public UsersController(UserService userService, PatientService patientService, LogService logService) {
            this._userUserService = userService;
            this._patientService = patientService;
            this._logService = logService;
        }

        // GET: api/Users
        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() {
            return await this._userUserService.GetAllUsersAsync();
        }

        // GET: api/Users/id
        [Authorize(Roles = "admin")]
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

        /*
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
        */

        // Post: api/Users/backoffice/create-patient
        [Authorize(Roles = "admin")]
        [HttpPost("backoffice/create-patient")]
        public async Task<ActionResult<PatientDto>> CreatePatient([FromBody] RegisterPatientDto patientDto) {
            try {
                var patient = await this._patientService.AddPatientAsync(patientDto);
                return Ok(patient);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }


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
        [Authorize(Roles = "admin")]
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

        //EditPatient:  api/Users/edit/id
        [Authorize(Roles = "admin")]
        [HttpPut("edit/{id}")]
        public async Task<ActionResult<PatientDto>> EditPatientProfile(string id,
            [FromBody] EditPatientDto editPatientDto) {
            try {
                var patientDto = await _patientService.EditPatientAsync(id, editPatientDto);
                return Ok(patientDto);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // DELETE: api/Users/admin/delete-patient
        [Authorize(Roles = "admin")]
        [HttpDelete("admin/delete-patient")]
        public async Task<ActionResult> DeletePatient([FromBody] AdminPatientDeletionDto dto) {
            try {
                var result = await _patientService.RequestAccountDeletionWithoutEmail(dto.Email);
                if (!result) {
                    return BadRequest(new { Message = "Failed to initiate patient deletion process" });
                }

                var user = await _userUserService.GetUserByEmailAsync(dto.Email);
                if (user == null) {
                    return NotFound(new { Message = "Patient not found" });
                }

                await _patientService.ConfirmAccountDeletionByAdmin(user.Id.Value);

                return Ok(new {
                    Message = "Patient deletion completed successfully",
                    Details = new {
                        PatientEmail = dto.Email,
                        DeletedAt = DateTime.UtcNow,
                        DeletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    }
                });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // GET: api/Users/admin/get-patients
        [Authorize(Roles = "admin")]
        [HttpGet("admin/get-patients")]
        public async Task<ActionResult<object>> GetPatientProfiles(
            [FromQuery] string searchTerm,
            [FromQuery] string searchBy,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10) {
            try {
                if (pageNumber < 1) pageNumber = 1;
                if (pageSize < 1) pageSize = 10;
                if (pageSize > 50) pageSize = 50;

                var (patients, totalCount) = await this._patientService.GetPatientProfilesAsync(
                    searchTerm,
                    searchBy,
                    pageNumber,
                    pageSize
                );

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                return Ok(new {
                    Data = patients ?? new List<UserDto>(),
                    Pagination = new {
                        CurrentPage = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount,
                        TotalPages = totalPages,
                        HasPrevious = pageNumber > 1,
                        HasNext = pageNumber < totalPages
                    },
                    SearchInfo = new {
                        SearchTerm = searchTerm,
                        SearchBy = searchBy
                    },
                    Message = patients.Count == 0 ? "No matching records found" : null
                });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
            catch (Exception ex) {
                Console.WriteLine($"Error in GetPatientProfiles: {ex}"); // Debug log
                return StatusCode(500, new { Message = "An error occurred while retrieving patient profiles" });
            }
        }

        // POST: api/Users/Patient/confirm-deletion
        [Authorize(Roles = "admin")]
        [HttpPost("Users/Admin/confirm-deletion")]
        public async Task<ActionResult> ConfirmDeletion([FromBody] ConfirmPatientDeletionDto dto) {
            try {
                var result = await _patientService.ConfirmAccountDeletion(dto.Token);
                if (!result) {
                    throw new BusinessRuleValidationException("Something failed");
                }

                return Ok(new { Message = "Account deletion process completed." });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // Inactivate: api/Users/inactivate/id
        [Authorize(Roles = "admin")]
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

        // Activate: api/Users/activate
        [Authorize(Roles = "admin,nurse,patient,doctor,technician")]
        [HttpPost("activate")]
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
        [Authorize(Roles = "nurse,doctor,technician")]
        [HttpGet("backoffice/request-password-reset")]
        public async Task<ActionResult> RequestPasswordReset() {
            try {
                var loggedUser = HttpContext.User;

                if (!loggedUser.Identity.IsAuthenticated) {
                    return Unauthorized();
                }
                var email = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                await this._userUserService.RequestPasswordResetAsync(email);
                return Ok("Password request sent to " + email);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users/backoffice/reset-password
        [Authorize(Roles = "nurse,doctor,technician")]
        [HttpPost("backoffice/reset-password")]
        public async Task<ActionResult<UserDto>> ResetPassword([FromBody] UserPasswordDto userPasswordDto) {
            try {
                var loggedUser = HttpContext.User;

                if (!loggedUser.Identity.IsAuthenticated) {
                    return Unauthorized();
                }

                var user = await _userUserService.CompletePasswordReset(userPasswordDto);
                return Ok(user);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users/Patient/signin
        [Authorize(Roles = "patient")]
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

        // POST: api/Users/Patient/request-deletion
        [Authorize(Roles = "patient")]
        [HttpPost("Patient/request-deletion")]
        public async Task<ActionResult> ResquestDeletion([FromBody] RequestAccountDeletionDto dto) {
            try {
                var result = await _patientService.RequestAccountDeletion(dto.Email);
                if (!result) {
                    throw new BusinessRuleValidationException("Something failed");
                }

                return Ok(new { Message = "Deletion request sent. Please check your email for confirmation." });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users/Patient/confirm-deletion
        [Authorize(Roles = "patient")]
        [HttpPost("Patient/confirm-deletion")]
        public async Task<ActionResult> ConfirmDeletion([FromBody] ConfirmAccountDeletionDto dto) {
            try {
                var result = await _patientService.ConfirmAccountDeletion(dto.Token);
                if (!result) {
                    throw new BusinessRuleValidationException("Something failed");
                }

                return Ok(new { Message = "Account deletion process completed." });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login([FromBody] LoginDto loginDto) {
            try {
                var user = await _userUserService.LoginAsync(loginDto);

                var claims = new List<Claim> {
                    new(ClaimTypes.Name, user.Username),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.NameIdentifier, user.Id.Value),
                    new(ClaimTypes.Role, user.Role.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)
                    });

                return Ok(new { Message = "Logged in successfully", UserId = user.Id.Value });
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // POST: api/Users/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(new { Message = "Logged out successfully" });
        }

        // POST: api/Users/Patient/edit-profile
        [Authorize(Roles = "patient")]
        [HttpPatch("Patient/edit-profile")]
        public async Task<ActionResult<PatientDto>> EditPatientProfile([FromBody] EditPatientDto dto) {
            try {
                var loggedUser = HttpContext.User;

                if (!loggedUser.Identity.IsAuthenticated) {
                    return Unauthorized();
                }

                var id = loggedUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var patient = await _patientService.EditPatientProfileAsync(id, dto);

                return Ok(patient);
            }
            catch (BusinessRuleValidationException e) {
                return BadRequest(new { Message = e.Message });
            }
        }

        // GET: api/Users/logs
        [Authorize(Roles = "admin")]
        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<LogDto>>> GetLogsAsync() {
            var logs = await this._logService.GetAllAsync();
            return Ok(logs);
        }


        // GET: api/Users/signin-google
        [HttpGet("signin-google")]
        [AllowAnonymous]
        public IActionResult SignInGoogle() {
            var properties = new AuthenticationProperties {
                RedirectUri = Url.Action(nameof(GoogleCallback))
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // GET: api/Users/signin-google-callback
        [HttpGet("signin-google-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback() {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest(new { Message = "Google authentication failed" });

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) {
                return BadRequest("Invalid email ->" + email);
            }

            var name = result.Principal.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(name)) {
                return BadRequest("Invalid name ->" + name);
            }

            try {
                UserDto existingUser = null;
                if (await _userUserService.UserExistsByEmailAsync(email)) {
                    existingUser = await _userUserService.GetUserByEmailAsync(email);
                }
                if (existingUser != null) {
                    var claims = new List<Claim> {
                        new(ClaimTypes.Name, existingUser.Username),
                        new(ClaimTypes.Email, existingUser.Email),
                        new(ClaimTypes.NameIdentifier, existingUser.Id.Value),
                        new(ClaimTypes.Role, existingUser.Role.ToString())
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties {
                            IsPersistent = true,
                            ExpiresUtc = DateTime.UtcNow.AddHours(1)
                        });

                    return Ok(new { Message = "Logged in successfully via Google", UserId = existingUser.Id.Value });
                }

                var registerDto = new RegisterPatientDto(
                    username: email.Split('@')[0],
                    email: email,
                    fullName: "full name",
                    phoneNumber: "966666666",
                    birthDate: DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    gender: "male",
                    emergencyContactFullName: "full name",
                    emergencyContactEmail: email,
                    emergencyContactPhoneNumber: "966666666",
                    medicalConditions: "None"
                );

                var patientDto = await _patientService.SignIn(registerDto);

                var newClaims = new List<Claim> {
                    new(ClaimTypes.Name, patientDto.User.Username),
                    new(ClaimTypes.Email, patientDto.User.Email),
                    new(ClaimTypes.NameIdentifier, patientDto.User.Id.Value),
                    new(ClaimTypes.Role, patientDto.User.Role.ToString())
                };

                var newClaimsIdentity =
                    new ClaimsIdentity(newClaims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(newClaimsIdentity),
                    new AuthenticationProperties {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddHours(1)
                    });

                return Ok(new {
                    Message = "Account created and logged in successfully via Google",
                    UserId = patientDto.User.Id.Value,
                    Note = "Please update your profile with complete information"
                });
            }
            catch (BusinessRuleValidationException ex) {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex) {
                return StatusCode(500, new { Message = "An error occurred during authentication: " + ex });
            }
        }

        // FALTAM OUTROS METODOS
    }
}