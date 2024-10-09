using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Infrastructure.EmailInfra;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUser {
    public class UserService {
        
        // ACRESCENTEM METODOS QUE PRECISEM DE LOGICA E DE MEXER COM OBJETOS DO DOMAIN
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IEmailService emailService) {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._emailService = emailService;
        }

        public async Task<List<UserDto>> GetAllUsersAsync() {
            var list = await this._repo.GetAllAsync();
            
            List<UserDto> listDto = list.ConvertAll(user =>
                new UserDto(
                    user.Id,
                    user.Active,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.Role));
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //listDTO.Add(new UserDto(new Guid(), true, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin));
            return listDto;
        }
        public async Task<ActionResult<UserDto>> GetUserByIdAsync(UserId id) {
            var user = await this._repo.GetByIdAsync(id);

            return user == null ? null : new UserDto(user);
        }

        public async Task<ActionResult<UserDto>> AddUserAsync(UserDto dto) {
            var userId = UserId.NewUserId();
            var user = new User(userId, dto.Username, dto.Email, dto.FullName, dto.PhoneNumber, dto.Role);
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //user = new User(userId, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin);
            await this._repo.AddAsync(user);
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName,
                user.PhoneNumber, user.Role);
        }

        public async Task<UserDto> InactivateUserAsync(UserId userId) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }
            
            user.MarkAsInative();
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }

        public async Task<UserDto> ActivateUserAsync(string token, string password) {
            
            Password.IsPasswordStrong(password);

            var user = await _repo.GetByActivationTokenAsync(token);

            if (user == null || user.ActivationTokenExpiry < DateTime.UtcNow) {
                throw new BusinessRuleValidationException("Invalid or expired token");
            }
            
            user.SetPassword(password);
            
            await _unitOfWork.CommitAsync();

            await _emailService.SendConfirmationEmailAsync(user.Email.Value);
            
            return new UserDto(user);
        }
        
        public async Task<UserDto> DeleteUserAsync(UserId userId) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }

            if (user.Active) {
                throw new BusinessRuleValidationException("It is not possible to delete an active user.");
            }
            
            this._repo.Remove(user);
            await this._unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }

        // TODO: Add some encryption to the password
        public async Task<UserDto> ResetPassword(UserId userId, string password) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }

            user.ChangePassword(password);

            await this._unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }
        
        public async Task<UserDto> CreateBackofficeUserAsync(CreateBackofficeUserDto dto) {
            var existingUser = await _repo.GetByEmailAsync(new Email(dto.Email));
            if (existingUser != null) {
                throw new BusinessRuleValidationException("Email already registered");
            }

            if (!IsValidBackofficeRole(dto.Role)) {
                throw new BusinessRuleValidationException("Invalid role for backoffice user");
            }
            var user = User.CreateBackofficeUser(dto);

            await _repo.AddAsync(user);
            await _unitOfWork.CommitAsync();

            await _emailService.SendActivationEmailAsync(user.Email.Value, user.ActivationToken);
            return new UserDto(user);
        }
        
        private static bool IsValidBackofficeRole(string role)
        {
            var validRoles = new[] { "admin", "doctor", "nurse", "technician" };
            return validRoles.Contains(role.ToLower());
        }
    }
}