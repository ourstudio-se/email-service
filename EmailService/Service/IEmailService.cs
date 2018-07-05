using System.Threading.Tasks;
using EmailService.Models;

namespace EmailService.Service
{
	public interface IEmailService
	{
		Task<string> SendEmailAsync(Email email);
	}
}