using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace ŠišAppApi.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrEmpty(toEmail))
                throw new ArgumentException("E-mail adresa ne može biti prazna.", nameof(toEmail));

            if (string.IsNullOrEmpty(subject))
                throw new ArgumentException("Predmet e-maila ne može biti prazan.", nameof(subject));

            if (string.IsNullOrEmpty(body))
                throw new ArgumentException("Sadržaj e-maila ne može biti prazan.", nameof(body));

            // Provjeri konfiguraciju
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderPassword = _configuration["EmailSettings:SenderPassword"];
            var smtpServerAddress = _configuration["EmailSettings:SmtpServer"];
            var smtpPortStr = _configuration["EmailSettings:SmtpPort"];

            if (string.IsNullOrEmpty(senderEmail))
                throw new InvalidOperationException("SenderEmail nije konfiguriran u appsettings.json");
            if (string.IsNullOrEmpty(senderPassword))
                throw new InvalidOperationException("SenderPassword nije konfiguriran u appsettings.json");
            if (string.IsNullOrEmpty(smtpServerAddress))
                throw new InvalidOperationException("SmtpServer nije konfiguriran u appsettings.json");
            if (string.IsNullOrEmpty(smtpPortStr))
                throw new InvalidOperationException("SmtpPort nije konfiguriran u appsettings.json");

            Console.WriteLine($"Pokušavam se spojiti na SMTP server: {smtpServerAddress}:{smtpPortStr}");

            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("ŠišApp", senderEmail));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = body
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new SmtpClient();
                
                Console.WriteLine("Povezivanje na SMTP server...");
                await smtp.ConnectAsync(smtpServerAddress, int.Parse(smtpPortStr), SecureSocketOptions.StartTls);
                Console.WriteLine("Uspješno povezivanje na SMTP server.");

                Console.WriteLine("Autentifikacija...");
                await smtp.AuthenticateAsync(senderEmail, senderPassword);
                Console.WriteLine("Uspješna autentifikacija.");

                Console.WriteLine("Slanje e-maila...");
                await smtp.SendAsync(email);
                Console.WriteLine("E-mail uspješno poslan.");

                Console.WriteLine("Odspajanje od SMTP servera...");
                await smtp.DisconnectAsync(true);
                Console.WriteLine("Uspješno odspajanje od SMTP servera.");

                Console.WriteLine($"E-mail uspješno poslan na: {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška prilikom slanja e-maila: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }
                throw; // Propagiram grešku dalje da je možemo uhvatiti u controlleru
            }
        }
    }
} 