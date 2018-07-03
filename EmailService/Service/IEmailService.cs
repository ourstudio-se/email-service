using System.Threading.Tasks;
using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service
{
	public interface IEmailService
	{
		Task<string> SendEmailAsync(Email email);
	}
}