﻿using System.Threading.Tasks;

namespace Sem5Pi2425.Domain.EmailAggr;

public interface IEmailService {
    Task SendEmailAsync(string to, string from, string subject, string body);
    Task SendConfirmationEmailAsync(string emailValue);
    Task SendActivationEmailAsync(string emailValue, string userActivationToken);
    Task SendBackofficeUserPasswordResetEmailAsync(string emailValue, string userPasswordRequestToken);
    Task SendWelcomeEmailAsync(string emailValue);
}