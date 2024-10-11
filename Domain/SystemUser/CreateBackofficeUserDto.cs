using Microsoft.Extensions.FileProviders;

namespace Sem5Pi2425.Domain.SystemUser;

public class CreateBackofficeUserDto {
    public string Username { set; get; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    
}