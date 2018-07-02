using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service
{
	public interface IEmailService
	{
		void SendEmail(EmailProperties emailProperties, IEmailServiceConfiguration configuration, Email email);
	}
}