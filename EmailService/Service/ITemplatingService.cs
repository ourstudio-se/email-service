using EmailService.Models;

namespace EmailService.Service
{
	public interface ITemplatingService
	{
		Template GetTemplate(string templateName);
	}
}