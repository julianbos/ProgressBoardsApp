using ProgressBoardsServices.Interfaces;
using System.Net.Mail;
using System.Net;

namespace ProgressBoardsServices.Implementations
{
	public class EmailService : IEmailService
	{
		private readonly string _smtpServer;
		private readonly int _smtpPort;
		private readonly string _fromEmail;
		private readonly string _smtpUser;
		private readonly string _smtpPass;

		public EmailService(string smtpServer, int smtpPort, string fromEmail, string smtpUser, string smtpPass)
		{
			_smtpServer = smtpServer;
			_smtpPort = smtpPort;
			_fromEmail = fromEmail;
			_smtpUser = smtpUser;
			_smtpPass = smtpPass;
		}

		public async Task SendEmailAsync(string toEmail, string subject, string message)
		{
			var client = new SmtpClient(_smtpServer, _smtpPort)
			{
				Credentials = new NetworkCredential(_smtpUser, _smtpPass),
				EnableSsl = true
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_fromEmail),
				Subject = subject,
				Body = message,
				IsBodyHtml = true
			};

			mailMessage.To.Add(toEmail);

			await client.SendMailAsync(mailMessage);
		}
	}
}
