using EmailService.Models;
using EmailService.Properties;
using EmailService.Utils;
using Newtonsoft.Json.Linq;

namespace EmailService.Service.Implementations
{
	public class SendGridEmailServiceDefinition : IEmailServiceDefinition
	{
		public string GetUrl()
		{
			return "https://api.sendgrid.com/v3/mail/send";
		}

		public string GetBody(EmailProperties emailProperties, Email email)
		{
			string contentType = ContentTypeUtility.GetContentTypeString(email.ContentType);
			
			return new SendGridBodyGenerator().Generate(email.To.ToString(), email.Subject, emailProperties.FromName,
				emailProperties.FromAddress, contentType, email.Content);
		}

		public string GetAuthenticationHeaderKey()
		{
			return "Authorization";
		}

		public string GetAuthenticationHeaderValue(EmailProperties emailProperties)
		{
			return $"Bearer {emailProperties.EmailServiceApiKey}";
		}
	}

	class SendGridBodyGenerator
	{
		/*
		 * Format:
		 *
		 * {
			 "personalizations": [
			   {
				 "to": [
				   {
					 "email": "john@example.com"
				   }
				 ],
				 "subject": "Hello, World!"
			   }
			 ],
			 "from": {
			   "email": "from_address@example.com"
			 },
			 "content": [
			   {
				 "type": "text/plain",
				 "value": "Hello, World!"
			   }
			 ]
		   }
		 */
		
		public string Generate(string receiverEmail, string subject, string fromName, string fromEmail,
			string emailContentType, string emailContent)
		{
			SendGridReceiver receiver = new SendGridReceiver() { Email = receiverEmail };
			
			SendGridReceiver[] receiverArray = new[] { receiver };
			
			SendGridPersonalization personalization = new SendGridPersonalization() { Subject = subject, To = receiverArray};
			SendGridContent content = new SendGridContent() { Type = emailContentType, Value = emailContent };
			SendGridSender sender = new SendGridSender() { Name = fromName, Email = fromEmail };
			
			SendGridPersonalization[] personalizations = { personalization };
			SendGridContent[] contentArray = { content };
			
			SendGridBody body = new SendGridBody() { Personalizations = personalizations, From = sender, Content = contentArray};

			return JObject.FromObject(body).ToString();
		}
	}

	class SendGridBody
	{
		public SendGridPersonalization[] Personalizations { get; set; }
		public SendGridSender From { get; set; }
		public SendGridContent[] Content { get; set; }
	}

	class SendGridPersonalization
	{
		public string Subject { get; set; }
		public SendGridReceiver[] To { get; set; }
	}

	class SendGridReceiver
	{
		public string Email { get; set; }
	}

	class SendGridSender
	{
		public string Name { get; set; }
		public string Email { get; set; }
	}

	class SendGridContent
	{
		public string Type { get; set; }
		public string Value { get; set; }
	}
}