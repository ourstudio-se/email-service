using EmailService.Configurations;

namespace EmailService.Utils
{
	public static class ServiceConfigurationUtility
	{
		public static ServiceConfiguration.EmailService ParseEmailService(string emailService)
		{
			switch (emailService)
			{
				case "sendgrid":
					return ServiceConfiguration.EmailService.SENDGRID;
				default:
					return ServiceConfiguration.EmailService.INVALID;
			}
		}

		public static ServiceConfiguration.LoggingType ParseLoggingType(string loggingType)
		{
			switch (loggingType)
			{
				case "api":
					return ServiceConfiguration.LoggingType.API;
				case "database":
					return ServiceConfiguration.LoggingType.DATABASE;
				case "none":
					return ServiceConfiguration.LoggingType.NONE;
				default:
					return ServiceConfiguration.LoggingType.INVALID;
			}
		}
	}
}