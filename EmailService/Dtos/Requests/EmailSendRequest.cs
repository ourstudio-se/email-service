using Newtonsoft.Json.Linq;

namespace EmailService.Dtos.Requests
{
	public class EmailSendRequest
	{
		public string[] To { get; set; }
		public string Template { get; set; }
		
		// GDPR-sensitive content, will be obfuscated in logs
		public JObject PersonalContent { get; set; }
		
		// Non-GDPR-sensitive content, will be logged
		public JObject Content { get; set; }
	}
}