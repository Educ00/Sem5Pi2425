namespace Sem5Pi2425.Domain.SystemUserAggr {
    public class UserDto {
        public UserId Id { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        
        public UserDto(User user) : this(user.Id, user.Active, user.Username.Value, user.Email, user.FullName.Value, user.PhoneNumber.Value, user.Role.ToString()) {}

        public UserDto(UserId id, bool active, string username, string email, string fullName, string phoneNumber, string role)
        {
            this.Id = id;
            this.Active = active;
            this.Username = username;
            this.Email = email;
            this.FullName = fullName;
            this.PhoneNumber = phoneNumber;
            this.Role = role;
        }
    }
}