using System.IO;
using EmailService.Models;
using EmailService.Utils;

namespace EmailService.Properties
{
	public class EmailProperties
	{
		public string EmailService { get; set; }
		public string EmailServiceUrl { get; set; }
		public string EmailServiceApiKey { get; set; }
		
		public string FromAddress { get; set; }
		public string FromName { get; set; }
		
		public Template[] Templates { get; set; }

		private const string EMAIL_CONFIGURATION_PATH = "Properties/emailProperties.json";

		private EmailProperties()
		{
		}

		public static EmailProperties CreateInstance()
		{
			string fullPath = Path.GetFullPath(EMAIL_CONFIGURATION_PATH);
			return FilesystemUtility.ReadJson<EmailProperties>(fullPath);
		}
	}
}