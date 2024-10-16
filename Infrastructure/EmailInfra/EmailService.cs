using System;
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
                var host = _configuration["Email:SmtpHost"] ?? "frodo.dei.isep.ipp.pt";
                var port = int.Parse(_configuration["Email:Port"] ?? "25");
                var useSsl = port != 25; // Assume SSL for ports other than 25

                _smtpClient = new SmtpClient(host, port) {
                    EnableSsl = useSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                Console.WriteLine($"SMTP client configured. Host: {host}, Port: {port}, UseSSL: {useSsl}");
                return true;
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to configure SMTP client: {ex.Message}");
                return false;
            }
        }

        public async Task SendEmailAsync(string to, string from, string subject, string body) {
            if (!_isConfigured) {
                Console.WriteLine($"Email not sent to {to}. Email service is not configured.");
                return;
            }

            var mailMessage = new MailMessage {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            try {
                Console.WriteLine($"Attempting to send email to {to}");
                await _smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine($"Email sent successfully to {to}");
            }
            catch (Exception ex) {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task SendActivationEmailAsync(string email, string activationToken) {
            var activationLink = $"{_configuration["AppUrl"]}/api/Users/activate?token={activationToken}";
            var subject = "Activate Your Account";
            var body = GetActivationEmailTemplate(activationLink, activationToken);
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(email, fromAddress, subject, body);
        }

        public async Task SendConfirmationEmailAsync(string email) {
            var subject = "Account Activated Successfully";
            var body = GetConfirmationEmailTemplate();
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(email, fromAddress, subject, body);
        }

        public async Task SendBackofficeUserPasswordResetEmailAsync(string emailValue, string userPasswordRequestToken) {
            var resetLink = $"{_configuration["AppUrl"]}/api/Users/backoffice/reset-password?token={userPasswordRequestToken}";
            var subject = "Reset Your Password";
            var body = GetPasswordResetEmailTemplate(resetLink);
            var fromAdress = _configuration["Email:FromAdress"];

            await SendEmailAsync(emailValue, fromAdress, subject, body);
        }

        private string GetConfirmationEmailTemplate() {
            return $@"
                <html>
                <body>
                    <h2>Welcome to Our System</h2>
                    <p>Your account has been successfully activated!</p>
                    <p>You can now log in and start using our services.</p>
                    <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                    <p>Thank you for joining us!</p>
                </body>
                </html>";
        }

        private string GetActivationEmailTemplate(string activationLink, string activationToken) {
            return $@"
                <html>
                <body>
                    <h2>Welcome to Our System</h2>
                    <p>Please click the link below to activate your account:</p>
                    <p><a href='{activationLink}'>Activate Account</a></p>
                    <p>Token->{activationToken}</p>
                    <p>This link will expire in 24 hours.</p>
                </body>
                </html>";
        }

        private string GetPasswordResetEmailTemplate(string resetLink) {
            return $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>You have requested to reset your password. Please click the link below to set a new password:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This link will expire in 24 hours.</p>
                    <p>If you did not request a password reset, please ignore this email.</p>
                </body>
                </html>";
        }
    }
}