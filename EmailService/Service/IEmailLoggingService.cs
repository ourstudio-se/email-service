using System.Threading.Tasks;
using EmailService.Properties;
using Newtonsoft.Json.Linq;

namespace EmailService.Service
{
	public interface IEmailLoggingService
	{
		Task LogAsync(string emailServiceId, string[] receivers, string template, JObject personalContent,
			JObject content);
	}
}