﻿namespace Sem5Pi2425.Domain.SystemUser {
    public class UserDto {
        public UserId Id { get; set; }
        public bool Active { get; set; }
        public Username Username { get; set; }
        public Email Email { get; set; }
        public FullName FullName { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public Role Role { get; set; }

        public UserDto(User user)
        {
            new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber, user.Role);
        }
        
        // Não sei ao certo o porquê, mas este dá com este post:
        /*
         * {
            "active": true,
            "username": {
                "value": "skrskr"
            },
            "email": {
                "value": "johndoe@example.com"
            },
            "fullName": {
                "value": "Joaquim Da Silva Queiros"
            },
            "phoneNumber": {
                "Value": "969999999"
            },
            "role": "Admin"
        }
        */
        /*
        [JsonConstructor]
        public UserDto(bool active, Username username, Email email, FullName fullName, PhoneNumber phoneNumber, Role role) {
            Id = new Guid();
            Active = active;
            Username = username;
            Email = email;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Role = role;
        }
        */
        
        public UserDto(UserId id, bool active, Username username, Email email, FullName fullName, PhoneNumber phoneNumber, Role role) {
            Id = id;
            Active = active;
            Username = username;
            Email = email;
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Role = role;
        }
    }
}