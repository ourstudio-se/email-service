using System.IO;
using EmailService.Utils;
using Newtonsoft.Json.Linq;

namespace EmailService.Configuration
{
	public class EmailConfiguration
	{
		public string FromAddress { get; set; }
		public string FromName { get; set; }
		public string TemplatePath { get; set; }
		public TemplateDefinition[] Templates { get; set; }

		private const string EMAIL_CONFIGURATION_PATH = "EmailConfiguration.json";

		private EmailConfiguration()
		{
		}

		public static EmailConfiguration CreateInstance()
		{
			string fullPath = Path.GetFullPath(EMAIL_CONFIGURATION_PATH);
			return FilesystemUtility.ReadJson<EmailConfiguration>(fullPath);
		}
	}
}