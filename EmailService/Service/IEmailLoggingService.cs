using System;
using System.Threading.Tasks;
using EmailService.Models;
using Newtonsoft.Json.Linq;

namespace EmailService.Service
{
	public interface IEmailLoggingService
	{
		Task<LogEntry> GetAsync(Guid id);
		Task LogAsync(string emailServiceId, string[] receivers, string template, JObject personalContent,
			JObject content);
	}
}