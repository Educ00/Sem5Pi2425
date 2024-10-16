﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.AppointmentAggr;
using Sem5Pi2425.Domain.PatientAggr;
using Sem5Pi2425.Domain.EmailAggr;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class UserService {
        // ACRESCENTEM METODOS QUE PRECISEM DE LOGICA E DE MEXER COM OBJETOS DO DOMAIN

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;
       // private readonly ICurrentUserService _currentUserService;
        private readonly IPatientRepository _patientrepo;
        
        
        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IEmailService emailService, IPatientRepository patientrepo) {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._emailService = emailService;
            this._patientrepo = patientrepo;
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

        public async Task<ActionResult<PatientDto>> AddPatientAsync(UserDto userDto, PatientDto patientDto) {
            var patientId = UserId.NewUserId();
            var patientUser = new User(patientId, userDto.Username, userDto.Email, userDto.FullName, userDto.PhoneNumber, Role.patient);

            var patient = new Patient(
                patientUser, 
                patientDto.EmergencyContact, 
                patientDto.MedicalConditions, 
                patientDto.BirthDate, 
                patientDto.Gender, 
                new List<Appointment>() 
            );

            await this._patientrepo.AddAsync(patient);
            await this._unitOfWork.CommitAsync();
            
            return new PatientDto(new UserDto(patient.User),patient.EmergencyContact, patient.MedicalConditions, patient.BirthDate, patient.Gender,new List<Appointment>()              
            );
        }

        public async Task<ActionResult<UserDto>> InactivateUserAsync(string userId) {
            var user = await this._repo.GetByIdAsync(new UserId(userId));

            if (user == null) {
                return null;
            }

            user.MarkAsInative();
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }

        public async Task<ActionResult<UserDto>> ActivateUserAsync(UserPasswordDto passwordDto) {
            if (passwordDto.Password != passwordDto.ConfirmPassword) {
                throw new BusinessRuleValidationException("Passwords do not match");
            }

            Password.IsPasswordStrong(passwordDto.Password);

            var user = await _repo.GetByActivationTokenAsync(passwordDto.Token);

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

        public async Task<ActionResult<UserDto>> CreateBackofficeUserAsync(CreateBackofficeUserDto dto) {
            var existingUser = await _repo.GetByEmailAsync(new Email(dto.Email));
            if (existingUser != null) {
                throw new BusinessRuleValidationException("Email already registered");
            }

            if (!IsValidBackofficeRole(dto.Role)) {
                throw new BusinessRuleValidationException("Invalid role for backoffice user");
            }

            var user = User.CreateBackofficeUser(dto);

            user = await _repo.AddAsync(user);
            await _emailService.SendActivationEmailAsync(user.Email.Value, user.ActivationToken);
            await _unitOfWork.CommitAsync();

            return new UserDto(user);
        }

        private static bool IsValidBackofficeRole(string role) {
            var validRoles = new[] { "admin", "doctor", "nurse", "technician" };
            return validRoles.Contains(role.ToLower());
        }
        
        public async Task<UserDto> GetUserByEmailAsync(string email) {
            var user = await _repo.GetByEmailAsync(email);
            Console.WriteLine("\n\n--------\nuser->" + user.Email + "<-\n");
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            return new UserDto(user);
        }

        public async Task<UserDto> GetBackofficeUserByEmailAsync(Email email) {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            if (!IsValidBackofficeRole(user.Role.ToString())) {
                throw new BusinessRuleValidationException("Invalid role for backoffice user");
            }

            return new UserDto(user);
        }

        public async Task RequestPasswordResetAsync(string email) {
            var user = await _repo.GetByEmailAsync(new Email(email));
            if (user == null) {
                throw new BusinessRuleValidationException("User not found");
            }

            user.GeneratePasswordRequestToken();
            await _emailService.SendBackofficeUserPasswordResetEmailAsync(user.Email.Value, user.PasswordRequestToken);
        }

        public async Task<UserDto> CompletePasswordReset(UserPasswordDto dto) {
            // TODO: Add some encryption to the password

            var user = await this._repo.GetByPasswordResetToken(dto.Token);

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
            var user = await _repo.GetByUsername(loginDto.Username);

            if (user == null) {
                throw new BusinessRuleValidationException("Invalid username or password");
            }

            if (!VerifyPassword(loginDto.Password, user.Password.Value)) {
                throw new BusinessRuleValidationException("Invalid username or password");
            }

            return new UserDto(user);
        }

        private bool VerifyPassword(string loginDtoPassword, string userPassword) {
            return loginDtoPassword.Equals(userPassword);
        }
    }
}