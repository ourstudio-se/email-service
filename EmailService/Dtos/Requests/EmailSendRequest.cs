using Newtonsoft.Json.Linq;

namespace EmailService.Dtos.Requests
{
	public class EmailSendRequest
	{
		public string To { get; set; }
		public string Template { get; set; }
		public JObject Content { get; set; }
	}
}