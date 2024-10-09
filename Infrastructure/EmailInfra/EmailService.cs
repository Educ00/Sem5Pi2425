using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sem5Pi2425.Infrastructure.EmailInfra {
    public class EmailService : IEmailService {
        private readonly IConfiguration _configuration;
        private SmtpClient _smtpClient;
        private readonly bool _isConfigured;

        public EmailService(IConfiguration configuration) {
            _configuration = configuration;
            _isConfigured = TryConfigureSmtpClient();
        }

        private bool TryConfigureSmtpClient() {
            try {
                var host = _configuration["Email:SmtpHost"];
                var port = int.Parse(_configuration["Email:Port"]);
                var username = _configuration["Email:Username"];
                var password = _configuration["Email:Password"];
                var useSSL = port != 25; // Assume SSL for ports other than 25

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) {
                    Console.WriteLine("Email configuration is incomplete. Check your appsettings.json file.");
                    return false;
                }

                _smtpClient = new SmtpClient(host, port) {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = useSSL,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                Console.WriteLine($"SMTP client configured. Host: {host}, Port: {port}, UseSSL: {useSSL}");
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to configure SMTP client: {ex.Message}");
                return false;
            }
        }

        public async Task SendActivationEmailAsync(string email, string activationToken) {
            if (!_isConfigured) {
                Console.WriteLine($"Activation email not sent to {email}. Email service is not configured.");
                return;
            }

            var activationLink = $"{_configuration["AppUrl"]}/activate?token={activationToken}";

            var mailMessage = new MailMessage {
                From = new MailAddress(_configuration["Email:FromAddress"]),
                Subject = "Activate Your Account",
                Body = GetActivationEmailTemplate(activationLink),
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            try {
                Console.WriteLine($"Attempting to send activation email to {email}");
                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Activation email sent successfully to {email}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to send activation email: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public Task SendConfirmationEmailAsync(string email) {
            throw new NotImplementedException();
        }

        // ... other methods ...

        private string GetActivationEmailTemplate(string activationLink) {
            return $@"
                <html>
                <body>
                    <h2>Welcome to Our System</h2>
                    <p>Please click the link below to activate your account:</p>
                    <p><a href='{activationLink}'>Activate Account</a></p>
                    <p>This link will expire in 24 hours.</p>
                </body>
                </html>";
        }
    }
}