using System;
using EmailService.Configuration;
using EmailService.Models;

namespace EmailService.Service.Implementations
{
	public class SendGridEmailService : IEmailService
	{
		public void SendEmail(EmailConfiguration emailConfiguration, IEmailServiceConfiguration configuration, Email email)
		{
			Console.WriteLine("Email: " + email.Content);
		}
	}
}