using System;
using Newtonsoft.Json.Linq;

namespace EmailService.Models
{
	public class LogEntry
	{
		public Guid Id { get; set; }
		public string EmailServiceId { get; set; }
		
		public DateTime Timestamp { get; set; }
		
		public string[] To { get; set; }
		public string Template { get; set; }
		
		public string PersonalContent { get; set; }
		public string Content { get; set; }
	}
}