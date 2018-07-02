using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service
{
	public interface IEmailServiceDefinition
	{
		string GetUrl();
		string GetBody(EmailProperties emailProperties, Email email);

		string GetAuthenticationHeaderKey();
		string GetAuthenticationHeaderValue(EmailProperties emailProperties);
	}
}