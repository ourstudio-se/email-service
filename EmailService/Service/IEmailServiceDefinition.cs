using System.Net.Http;
using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service
{
	public interface IEmailServiceDefinition
	{
		string GetBody(EmailProperties emailProperties, Email email);
		string GetAuthenticationHeaderScheme();
		string GetAuthenticationHeaderValue(ServiceProperties serviceProperties);
		string GetIdFromResponse(HttpResponseMessage response);
	}
}