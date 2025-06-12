namespace Cvjecara_Latica.Services
{
    using System.Net;
    using System.Net.Mail;
    using Microsoft.Extensions.Configuration;
    using System.Threading.Tasks;

    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var smtpClient = new SmtpClient(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]))
            {
                Credentials = new NetworkCredential(_config["EmailSettings:Username"], _config["EmailSettings:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };

            mail.To.Add(toEmail);
            await smtpClient.SendMailAsync(mail);
        }
    }
}

