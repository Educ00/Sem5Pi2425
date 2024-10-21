using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sem5Pi2425.Domain.EmailAggr {
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
                Console.WriteLine($"Failed to configure SMTP client, maybe connect to ISEP DEI with a vpn: {ex.Message}");
                return false;
            }
        }

        public async Task SendEmailAsync(string to, string from, string subject, string body) {
            if (!_isConfigured) {
                Console.WriteLine($"Email not sent to {to}. Email service is not configured.");
                return;
            }

            if (!to.EndsWith("@isep.ipp.pt")) {
                Console.WriteLine($"Email could not be sent to {to} because it is not a email from isep. The email is valid tho.");
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

        public async Task
            SendBackofficeUserPasswordResetEmailAsync(string emailValue, string userPasswordRequestToken) {
            var resetLink =
                $"{_configuration["AppUrl"]}/api/Users/backoffice/reset-password?token={userPasswordRequestToken}";
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

        public async Task SendWelcomeEmailAsync(string email) {
            var subject = "Welcome to Our Healthcare Application";
            var body = GetWelcomeEmailTemplate();
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(email, fromAddress, subject, body);
        }

        public async Task SendAccountDeletionConfirmationEmailAsync(string emailValue, string deletionToken) {
            var subject = "Confirm Account Deletion";
            var body = GetAccountDeletionConfirmationEmailTemplate(deletionToken);
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(emailValue, fromAddress, subject, body);
        }

        public async Task SendAccountDeletionCompletedEmailAsync(string email) {
            var subject = "Account Deletion Completed";
            var body = GetAccountDeletionCompletedEmailTemplate();
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(email, fromAddress, subject, body);
        }

        public async Task SendAdminUserLockoutEmailAsync(List<string> emailList, string blockedUsername) {
            var subject = $"User Account Locked: {blockedUsername}";
            var body = GetAdminUserLockoutEmailTemplate(blockedUsername);
            var fromAddress = _configuration["Email:FromAddress"];

            foreach (var adminEmail in emailList) {
                try {
                    await SendEmailAsync(adminEmail, fromAddress, subject, body);
                    Console.WriteLine($"Admin lockout notification sent to {adminEmail}");
                }
                catch (Exception ex) {
                    Console.WriteLine($"Failed to send admin lockout notification to {adminEmail}: {ex.Message}");
                }
            }
        }

        public async Task SendProfileChangedConfirmationEmailAsync(string emailValue) {
            var subject = "Profile Updated Confirmation";
            var body = GetProfileChangedConfirmationEmailTemplate();
            var fromAddress = _configuration["Email:FromAddress"];

            await SendEmailAsync(emailValue, fromAddress, subject, body);
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

        private string GetWelcomeEmailTemplate() {
            return @"
            <html>
            <body>
                <h2>Welcome to Our Healthcare Application!</h2>
                <p>Thank you for registering with us using your Google account.</p>
                <p>You can now use our application to book appointments and manage your healthcare needs.</p>
                <p>If you have any questions, please don't hesitate to contact our support team.</p>
            </body>
            </html>";
        }

        private string GetAccountDeletionConfirmationEmailTemplate(string deletionToken) {
            var confirmationLink = $"{_configuration["AppUrl"]}/api/Users/confirm-deletion?token={deletionToken}";
            return $@"
                <html>
                <body>
                    <h2>Confirm Account Deletion</h2>
                    <p>You have requested to delete your account. To confirm this action, please click the link below:</p>
                    <p><a href='{confirmationLink}'>Confirm Account Deletion</a></p>
                    <p>If you did not request this action, please ignore this email.</p>
                    <p>This link will expire in 30 days.</p>
                </body>
                </html>";
        }

        private string GetAccountDeletionCompletedEmailTemplate() {
            return @"
                <html>
                <body>
                    <h2>Account Deletion Completed</h2>
                    <p>Your account and associated data have been successfully deleted from our system.</p>
                    <p>We're sorry to see you go. If you have any feedback or concerns, please don't hesitate to contact our support team.</p>
                    <p>Thank you for using our services.</p>
                </body>
                </html>";
        }

        private string GetAdminUserLockoutEmailTemplate(string blockedUsername) {
            return $@"
        <html>
        <body>
            <h2>User Account Locked</h2>
            <p>This is an automated notification to inform you that a user account has been locked due to multiple failed login attempts.</p>
            <p><strong>Locked Username:</strong> {blockedUsername}</p>
            <p>The account has been temporarily locked after 5 unsuccessful login attempts.</p>
            <p>Please review this account and take appropriate action if necessary.</p>
            <p>If you have any questions or concerns, please contact the IT department.</p>
        </body>
        </html>";
        }
        
        private string GetProfileChangedConfirmationEmailTemplate() {
            return @"
                <html>
                <body>
                    <h2>Profile Update Confirmation</h2>
                    <p>Your profile has been successfully updated.</p>
                    <p>If you did not make this change, please contact our support team immediately.</p>
                    <p>Thank you for using our services!</p>
                </body>
                </html>";
        }
    }
}