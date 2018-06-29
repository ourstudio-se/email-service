namespace EmailService.Service.Implementations
{
	public class SendGridEmailConfiguration : IEmailConfiguration
	{
		public string GetUrl()
		{
			return "https://api.sendgrid.com/v3/mail/send";
		}
	}
}