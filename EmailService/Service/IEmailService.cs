using EmailService.Models;

namespace EmailService.Service
{
	public interface IEmailService
	{
		void SendEmail(IEmailConfiguration configuration, Email email);
	}
}