using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class User : Entity<UserId>, IAggregateRoot {
        public Email Email { get; private set; }
        public FullName FullName { get; private set; }
        public Role Role { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public bool Active { get; private set; }
        public Username Username { get; private set; }
        public Password Password { get; private set; }
        public string ActivationToken { get; private set; }
        public DateTime? ActivationTokenExpiry { get; private set; }
        public string PasswordRequestToken { get; private set; }
        public DateTime? PasswordRequestTokenExpiry { get; private set; }
        public string DeletionToken { get; private set; }
        public bool MarkedForDeletion { get; private set; }
        public int LoginAttempts { get; private set; }
        public bool IsLockedOut { get; private set; }
        public DateTime? LockoutEnd { get; private set; }

        protected User() { }

        public User(UserId userId, Username username, Email email, FullName fullName, PhoneNumber phoneNumber,
            Role role) {
            this.Id = userId;
            this.Active = false;
            this.Username = username;
            this.Email = email;
            this.FullName = fullName;
            this.PhoneNumber = phoneNumber;
            this.Role = role;
            this.ActivationToken = Guid.NewGuid().ToString();
            this.ActivationTokenExpiry = DateTime.UtcNow.AddHours(24);
            this.PasswordRequestToken = "";
            this.PasswordRequestTokenExpiry = null;
        }

        public static User CreateBackofficeUser(CreateBackofficeUserDto dto) {
            return new User(
                UserId.NewUserId(), new Username(dto.Username), new Email(dto.Email), new FullName(dto.FullName),
                new PhoneNumber(dto.PhoneNumber), SetBackofficeRoleFromString(dto.Role));
        }

        private static Role SetBackofficeRoleFromString(string role) {
            if (Enum.TryParse<Role>(role, true, out var parsedRole)) {
                if (parsedRole == Role.patient) {
                    throw new BusinessRuleValidationException("Patient is not a backoffice user!");
                }

                return parsedRole; // Successfully stored the role
            }
            else {
                throw new BusinessRuleValidationException("Invalid role!");
            }
        }

        public void SetPassword(string password) {
            if (this.Password != null) {
                throw new BusinessRuleValidationException("Password already set");
            }

            this.Password = new Password(password);
            this.ActivationToken = null;
            this.ActivationTokenExpiry = null;
            MarkAsActive();
        }

        public void UpdateFullName(FullName fullName)
        {
            FullName = fullName ;
        }

        public void UpdatePhoneNumber(PhoneNumber phoneNumber)
        {
            PhoneNumber = phoneNumber ;
        }

        public void UpdateEmail(Email email)
        {
            Console.WriteLine("Erro");
            Email = new Email(email);
        }
        
        // Alguns metodos que podem ser uteis para ativar e desativar utilizadores
        public void MarkAsInative() {
            this.Active = false;
        }

        public void MarkAsActive() {
            this.Active = true;
        }

        public void ChangePassword(string password) {
            this.Password = new Password(password);
            this.PasswordRequestToken = null;
            this.PasswordRequestTokenExpiry = null;
        }

        public void GeneratePasswordRequestToken() {
            this.PasswordRequestToken = Guid.NewGuid().ToString();
            this.PasswordRequestTokenExpiry = DateTime.UtcNow.AddHours(24);
        }

        public void SetDeletionToken(string token) {
            this.DeletionToken = token;
        }

        public void MarkForDeletion() {
            this.MarkedForDeletion = true;
            this.DeletionToken = null;
        }

        public void IncrementLoginAttempts() {
            this.LoginAttempts = this.LoginAttempts + 1;
        }

        public void BlockLogin(int lockoutDurationMinutes) {
            this.IsLockedOut = true;
            this.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutDurationMinutes);
        }

        public void ResetLoginAttempts() {
            this.LoginAttempts = 0;
            this.IsLockedOut = false;
            this.LockoutEnd = null;
        }

        public void UnblockLogin() {
            this.IsLockedOut = false;
            this.LockoutEnd = null;
            this.LoginAttempts = 0;
        }
    }
}