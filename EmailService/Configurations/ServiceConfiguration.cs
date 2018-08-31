using Newtonsoft.Json;

namespace EmailService.Configurations
{
	public class ServiceConfiguration
	{
		public const string CONFIGURATION_PATH = "app_content/serviceConfiguration.json";
		
		[JsonProperty("LoggingType")]
		public string LoggingTypeString { get; set; }
		[JsonProperty("EmailService")]
		public string EmailServiceString { get; set; }
		
		public LoggingType SelectedLoggingType { get; set; }
		public string LoggingApiUrl { get; set; }
		public string LoggingDatabaseConnectionString { get; set; }
		
		public EmailService SelectedEmailService { get; set; }
		public string EmailServiceUrl { get; set; }
		public string EmailServiceApiKey { get; set; }
		
		public string ServiceApiKey { get; set; }
		
		public enum LoggingType
		{
			API,
			DATABASE,
			NONE,
			INVALID
		}

		public enum EmailService
		{
			SENDGRID,
			INVALID
		}
	}
}