using System;
using System.Threading.Tasks;

namespace Sem5Pi2425.Infrastructure.EmailInfra;

public class DevelopmentEmailService : IEmailService
{
    public Task SendActivationEmailAsync(string email, string activationToken)
    {
        Console.WriteLine($"Development: Activation email sent to {email} with token {activationToken}");
        return Task.CompletedTask;
    }

    public Task SendConfirmationEmailAsync(string email)
    {
        Console.WriteLine($"Development: Confirmation email sent to {email}");
        return Task.CompletedTask;
    }
}
