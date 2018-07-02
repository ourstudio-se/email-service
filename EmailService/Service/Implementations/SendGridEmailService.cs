using System;
using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service.Implementations
{
	public class SendGridEmailService : IEmailService
	{
		public void SendEmail(EmailProperties emailProperties, IEmailServiceConfiguration configuration, Email email)
		{
			Console.WriteLine("Email: " + email.Content);
		}
	}
}