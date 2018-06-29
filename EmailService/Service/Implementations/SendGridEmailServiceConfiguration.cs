namespace EmailService.Service.Implementations
{
	public class SendGridEmailServiceConfiguration : IEmailServiceConfiguration
	{
		public string GetUrl()
		{
			return "https://api.sendgrid.com/v3/mail/send";
		}
	}
}