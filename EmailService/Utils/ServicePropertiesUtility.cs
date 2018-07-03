using EmailService.Properties;

namespace EmailService.Utils
{
	public static class ServicePropertiesUtility
	{
		public static ServiceProperties.EmailService? ParseEmailService(string emailService)
		{
			switch (emailService)
			{
				case "sendgrid":
					return ServiceProperties.EmailService.SENDGRID;
				default:
					return null;
			}
		}

		public static ServiceProperties.LoggingType? ParseLoggingType(string loggingType)
		{
			switch (loggingType)
			{
				case "api":
					return ServiceProperties.LoggingType.API;
				case "database":
					return ServiceProperties.LoggingType.DATABASE;
				case "none":
					return ServiceProperties.LoggingType.NONE;
				default:
					return null;
			}
		}
	}
}