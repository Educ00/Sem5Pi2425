namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class UserDto {
        public UserId Id { get; set; }
        public bool Active { get; set; }
        public Username Username { get; set; }
        public Email Email { get; set; }
        public FullName FullName { get; set; }
        public PhoneNumber PhoneNumber { get; set; }
        public Role Role { get; set; }
        
        public UserDto(User user) : this(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber, user.Role) {}

        public UserDto(UserId id, bool active, Username username, Email email, FullName fullName,
            PhoneNumber phoneNumber, Role role) {
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