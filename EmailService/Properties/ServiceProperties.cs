namespace EmailService.Properties
{
	public class ServiceProperties
	{
		public LoggingType SelectedLoggingType { get; set; }
		public string LoggingApiUrl { get; set; }
		public string LoggingDatabaseConnectionString { get; set; }
		
		public EmailService SelectedEmailService { get; set; }
		public string EmailServiceUrl { get; set; }
		public string EmailServiceApiKey { get; set; }
		
		public enum LoggingType
		{
			API,
			DATABASE,
			NONE
		}

		public enum EmailService
		{
			SENDGRID
		}
	}
}