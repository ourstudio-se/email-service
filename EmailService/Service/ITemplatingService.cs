using EmailService.Configuration;
using EmailService.Models;

namespace EmailService.Service
{
	public interface ITemplatingService
	{
		Template GetTemplate(EmailConfiguration emailConfiguration, string templateName);
	}
}