using System;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.Users {
    public class User : Entity<UserId>, IAggregateRoot {
        public bool Active { get; private set; }

        public Username Username { get; private set; }

        public Email Email { get; private set; }

        public FullName FullName { get; private set; }

        public PhoneNumber PhoneNumber { get; private set; }

        public Role Role { get; private set; }
        
        public Password Password { get; private set; }

        protected User() { }

        public User(Username username, Email email, FullName fullName, PhoneNumber phoneNumber, Role role) {
            this.Id = new UserId(Guid.NewGuid());
            this.Active = true;
            this.Username = username;
            this.Email = email;
            this.FullName = fullName;
            this.PhoneNumber = phoneNumber;
            this.Role = role;
            this.Password = new Password("12345678A@");
            // Este console log é so para lembrar da password, que neste momento vai ser 12345678A@ para todos os utilizadores criados
            Console.WriteLine("Password Created For user " + username.ToString() + "->" + this.Password);
        }

        // Alguns metodos que podem ser uteis para ativar e desativar utilizadores
        public void MarkAsInative() {
            this.Active = false;
        }

        public void MarkAsActive() {
            this.Active = true;
        }
    }
}