using Newtonsoft.Json.Linq;

namespace EmailService.Service
{
	public interface IEmailLoggingService
	{
		void Log(string emailServiceId, string[] receivers, string template, JObject personalContent, JObject content);
	}
}