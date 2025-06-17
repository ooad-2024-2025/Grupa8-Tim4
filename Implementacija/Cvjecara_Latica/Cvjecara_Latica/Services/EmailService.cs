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
        public async Task SendReminder(string toEmail)
        {
            string subject = "Reminder: You haven't picked up your order";
            string body = "<p>Dear customer,</p>" +
                          "<p>We noticed that your order hasn't been picked up yet. Please pick it up as soon as possible.</p>" +
                          "<p>If you fail to do so within the next 24 hours, your account may be suspended.</p>" +
                          "<p>Thank you,<br/>Cvjećara Latica</p>";

            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendDeactivationNotice(string toEmail)
        {
            string subject = "Account Deactivated - Unclaimed Order";
            string body = "<p>Dear customer,</p>" +
                          "<p>Your account has been deactivated because you did not pick up your order within 24 hours after the delivery date.</p>" +
                          "<p>If you believe this is a mistake, please contact our support team.</p>" +
                          "<p>Thank you,<br/>Cvjećara Latica</p>";

            await SendEmailAsync(toEmail, subject, body);
        }
    }
}

