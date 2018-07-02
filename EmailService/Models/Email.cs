using System.Net.Mail;

namespace EmailService.Models
{
	public class Email
	{
		public MailAddress[] To { get; set; }
		
		public string Subject { get; set; }
		
		public ContentType ContentType { get; set; }
		
		public string Content { get; set; }

		public Email(MailAddress[] to, string subject, ContentType contentType, string content)
		{
			To = to;
			Subject = subject;
			ContentType = contentType;
			Content = content;
		}
	}
}