namespace Sem5Pi2425.Domain.SystemUser;

public class ActivateUserDto
{
    public string Token { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}