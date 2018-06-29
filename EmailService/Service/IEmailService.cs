using EmailService.Models;

namespace EmailService.Service
{
	public interface IEmailService
	{
		void SendEmail(EmailConfiguration emailConfiguration, IEmailServiceConfiguration configuration, Email email);
	}
}