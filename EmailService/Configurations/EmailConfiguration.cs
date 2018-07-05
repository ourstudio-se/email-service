using EmailService.Models;

namespace EmailService.Configurations
{
	public class EmailConfiguration
	{
		public const string CONFIGURATION_PATH = "app_content/emailConfiguration.json";
		
		public string FromAddress { get; set; }
		public string FromName { get; set; }
		
		public Template[] Templates { get; set; }
	}
}