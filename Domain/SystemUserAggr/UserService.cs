using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.EmailAggr;
using Sem5Pi2425.Domain.Shared;
using Sem5Pi2425.Domain.StaffAggr;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class UserService {
        // ACRESCENTEM METODOS QUE PRECISEM DE LOGICA E DE MEXER COM OBJETOS DO DOMAIN

        private const int MaxLoginAttempts = 5;
        private const int LockoutDurationMinutes = 5;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepo;
        private readonly IEmailService _emailService;
        // private readonly ICurrentUserService _currentUserService;


        public UserService(IUnitOfWork unitOfWork, IUserRepository userRepo,
            IEmailService emailService) {
            this._unitOfWork = unitOfWork;
            this._userRepo = userRepo;
            this._emailService = emailService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync() {
            var list = await this._userRepo.GetAllAsync();

            List<UserDto> listDto = list.ConvertAll(user =>
                new UserDto(
                    user.Id,
                    user.Active,
                    user.Username.ToString(),
                    user.Email,
                    user.FullName.ToString(),
                    user.PhoneNumber.ToString(),
                    user.Role.ToString()));
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //listDTO.Add(new UserDto(new Guid(), true, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin));
            return listDto;
        }

        public async Task<ActionResult<UserDto>> GetUserByIdAsync(UserId id) {
            var user = await this._userRepo.GetByIdAsync(id);

            return user == null ? null : new UserDto(user);
        }

        public async Task<ActionResult<UserDto>> AddUserAsync(UserDto dto) {
            var userId = UserId.NewUserId();
            var username = new Username(dto.Username);
            var email = new Email(dto.Email);
            var fullname = new FullName(dto.FullName);
            var phoneNumber = new PhoneNumber(dto.PhoneNumber);
            Enum.TryParse(dto.Role, true, out Role role);
            var user = new User(userId, username, email, fullname, phoneNumber, role);
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //user = new User(userId, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin);
            await this._userRepo.AddAsync(user);
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username.ToString(), user.Email, user.FullName.ToString(),
                user.PhoneNumber.ToString(), user.Role.ToString());
        }


        public async Task<ActionResult<UserDto>> InactivateUserAsync(string userId) {
            var user = await this._userRepo.GetByIdAsync(new UserId(userId));

            if (user == null) {
                return null;
            }

            user.MarkAsInative();
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username.ToString(), user.Email, user.FullName.ToString(),
                user.PhoneNumber.ToString(),
                user.Role.ToString());
        }

        public async Task<ActionResult<UserDto>> ActivateUserAsync(UserPasswordDto passwordDto) {
            if (passwordDto.Password != passwordDto.ConfirmPassword) {
                throw new BusinessRuleValidationException("Passwords do not match");
            }

            Password.IsPasswordStrong(passwordDto.Password);

            var user = await _userRepo.GetByActivationTokenAsync(passwordDto.Token);

            if (user == null || user.ActivationTokenExpiry < DateTime.UtcNow) {
                throw new BusinessRuleValidationException("Invalid or expired token");
            }

            user.SetPassword(passwordDto.Password);

            await _unitOfWork.CommitAsync();

            await _emailService.SendConfirmationEmailAsync(user.Email.Value);

            return new UserDto(user);
        }

        /*
        public async Task<UserDto> GetCurrentUserAsync() {
            var userId = new UserId(_currentUserService.UserId);
            var user = await _repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }

            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber, user.Role);
        }

*/
        public async Task<ActionResult<UserDto>> DeleteUserAsync(UserId userId) {
            Console.WriteLine("USERID SERVICE->" + userId.Value);
            var user = await this._userRepo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }

            if (user.Active) {
                throw new BusinessRuleValidationException("It is not possible to delete an active user.");
            }

            this._userRepo.Remove(user);
            await this._unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Active, user.Username.Value, user.Email.Value, user.FullName.Value,
                user.PhoneNumber.Value,
                user.Role.ToString());
        }

        public async Task<ActionResult<UserDto>> CreateBackofficeUserAsync(CreateBackofficeUserDto dto) {
            var existingUser = await _userRepo.GetByEmailAsync(new Email(dto.Email));
            if (existingUser != null) {
                throw new BusinessRuleValidationException("Email already registered");
            }

            if (!IsValidBackofficeRole(dto.Role)) {
                throw new BusinessRuleValidationException("Invalid role for backoffice user");
            }

            var user = User.CreateBackofficeUser(dto);

            user = await _userRepo.AddAsync(user);
            await _emailService.SendActivationEmailAsync(user.Email.Value, user.ActivationToken);
            await _unitOfWork.CommitAsync();

            return new UserDto(user);
        }

        private static bool IsValidBackofficeRole(string role) {
            var validRoles = new[] { "admin", "doctor", "nurse", "technician" };
            return validRoles.Contains(role.ToLower());
        }

        public async Task<UserDto> GetUserByEmailAsync(string email) {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            return new UserDto(user);
        }

        public async Task<UserDto> GetBackofficeUserByEmailAsync(Email email) {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            if (!IsValidBackofficeRole(user.Role.ToString())) {
                throw new BusinessRuleValidationException("Invalid role for backoffice user");
            }

            return new UserDto(user);
        }

        public async Task RequestPasswordResetAsync(string email) {
            var user = await _userRepo.GetByEmailAsync(new Email(email));
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            user.GeneratePasswordRequestToken();
            await _unitOfWork.CommitAsync();
            await _emailService.SendBackofficeUserPasswordResetEmailAsync(user.Email.Value, user.PasswordRequestToken);
        }

        public async Task<UserDto> CompletePasswordReset(UserPasswordDto dto) {
            // TODO: Add some encryption to the password

            var user = await this._userRepo.GetByPasswordResetToken(dto.Token);

            if (user == null || user.PasswordRequestTokenExpiry < DateTime.UtcNow) {
                throw new BusinessRuleValidationException("Invalid or expired token.");
            }

            if (dto.Password != dto.ConfirmPassword) {
                throw new BusinessRuleValidationException("Passwords do not match!");
            }

            Password.IsPasswordStrong(dto.Password);

            user.ChangePassword(dto.Password);

            await this._unitOfWork.CommitAsync();

            return new UserDto(user);
        }

        public async Task<UserDto> LoginAsync(LoginDto loginDto) {
            var user = await _userRepo.GetByUsername(loginDto.Username);

            if (user == null) {
                throw new BusinessRuleValidationException("Invalid username or password");
            }


            await CheckAndResetLockout(user);

            if (!VerifyPassword(loginDto.Password, user.Password.Value)) {
                await IncrementInvalidLoginAttempts(user);
                throw new BusinessRuleValidationException("Invalid username or password");
            }


            await ResetLoginAttempts(user);
            return new UserDto(user);
        }

        private async Task CheckAndResetLockout(User user) {
            if (user.IsLockedOut) {
                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTime.UtcNow) {
                    user.UnblockLogin();
                    await _unitOfWork.CommitAsync();
                }
                else {
                    throw new BusinessRuleValidationException("Your account is blocked. Try again in " +
                                                              user.LockoutEnd);
                }
            }
        }

        private async Task IncrementInvalidLoginAttempts(User user) {
            user.IncrementLoginAttempts();
            if (!user.IsLockedOut && user.LoginAttempts >= MaxLoginAttempts) {
                user.BlockLogin(LockoutDurationMinutes);
                var adminList = await _userRepo.GetWithRoleAsync(Role.admin);
                var emailList = adminList.Select(u => u.Email.Value).ToList();
                await _emailService.SendAdminUserLockoutEmailAsync(emailList, user.Username.Value);
            }

            await _unitOfWork.CommitAsync();
        }

        private async Task ResetLoginAttempts(User user) {
            user.ResetLoginAttempts();
            await _unitOfWork.CommitAsync();
        }

        private bool VerifyPassword(string loginDtoPassword, string userPassword) {
            return loginDtoPassword.Equals(userPassword);
        }

        public async Task<bool> UserExistsByEmailAsync(string email) {
            var exists = false;
            try {
                var user = await _userRepo.GetByEmailAsync(email);
                if (user == null) {
                    throw new BusinessRuleValidationException("Unkown error!");
                }

                exists = true;
            }
            catch (BusinessRuleValidationException e) {
                if (e.Message.Equals("User not found")) {
                    exists = false;
                }
            }

            return exists;
        }
    }
}