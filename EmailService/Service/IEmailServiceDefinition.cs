using System.Net.Http;
using EmailService.Configurations;
using EmailService.Models;

namespace EmailService.Service
{
	public interface IEmailServiceDefinition
	{
		string GetBody(EmailConfiguration emailConfiguration, Email email);
		string GetAuthenticationHeaderScheme();
		string GetAuthenticationHeaderValue(ServiceConfiguration serviceConfiguration);
		string GetIdFromResponse(HttpResponseMessage response);
	}
}