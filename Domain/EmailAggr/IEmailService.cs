using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sem5Pi2425.Domain.EmailAggr;

public interface IEmailService {
    Task SendEmailAsync(string to, string from, string subject, string body);
    Task SendConfirmationEmailAsync(string emailValue);
    Task SendActivationEmailAsync(string emailValue, string userActivationToken);
    Task SendBackofficeUserPasswordResetEmailAsync(string emailValue, string userPasswordRequestToken);
    Task SendWelcomeEmailAsync(string emailValue);
    Task SendAccountDeletionConfirmationEmailAsync(string emailValue, string deletionToken);
    Task SendAccountDeletionCompletedEmailAsync(string emailValue);
    Task SendAdminUserLockoutEmailAsync(List<string> emailList, string blockedUsername);
    Task SendProfileChangedConfirmationEmailAsync(string emailValue);
}