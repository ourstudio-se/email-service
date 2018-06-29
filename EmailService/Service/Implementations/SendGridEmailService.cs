using EmailService.Models;

namespace EmailService.Service.Implementations
{
	public class SendGridEmailService : IEmailService
	{
		public void SendEmail(EmailConfiguration emailConfiguration, IEmailServiceConfiguration configuration, Email email)
		{
			throw new System.NotImplementedException();
		}
	}
}