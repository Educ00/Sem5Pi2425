using System.Threading.Tasks;

namespace Sem5Pi2425.Infrastructure.EmailInfra;

public interface IEmailService {
    Task SendActivationEmailAsync(string email, string activationToken);
    Task SendConfirmationEmailAsync(string email);
    Task SendBackofficeUserPasswordResetEmailAsync(string emailValue, string userPasswordRequestToken);
}