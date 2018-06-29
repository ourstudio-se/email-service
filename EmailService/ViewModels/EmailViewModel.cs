using Newtonsoft.Json.Linq;

namespace EmailService.ViewModels
{
	public class EmailViewModel
	{
		public string TemplateName { get; set; }
		public JObject Content { get; set; }
	}
}