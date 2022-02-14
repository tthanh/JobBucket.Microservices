using JB.Infrastructure.Constants;
using JB.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace JB.Authentication.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(
                        IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<Status> SendEmailAsync(string email, string subject, string message, bool isBodyHtml = true)
        {
            Status result = new Status();

            do
            {
                if (_configuration == null)
                {
                    result.ErrorCode = ErrorCode.ConfigurationNotFound;
                    break;
                }

                //Check arguments
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                {
                    result.ErrorCode = ErrorCode.InvalidArgument;
                    break;
                }

                //Get configuration for email sender
                var fromEmailAddress = _configuration.GetSection("Email:EmailAddress")?.Value;
                var password = _configuration.GetSection("Email:Password")?.Value;
                var host = _configuration.GetSection("Email:Host")?.Value;
                var canParsePort = int.TryParse(_configuration.GetSection("Email:Port")?.Value, out int port);
                if (string.IsNullOrEmpty(fromEmailAddress) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(host) || !canParsePort)
                {
                    result.ErrorCode = ErrorCode.ConfigurationNotFound;
                    break;
                }

                var fromAddress = new MailAddress(fromEmailAddress);
                var toAddress = new MailAddress(email);

                //Init email client
                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, password)
                };

                try
                {
                    using var mail = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = subject,
                        Body = message,
                        IsBodyHtml = true
                    };
                    await smtp.SendMailAsync(mail);
                }
                catch (Exception e)
                {
                    result.ErrorCode = ErrorCode.UnableToConnectToEmailHost;
                    break;
                }
            }
            while (false);

            return result;
        }
    }
}
