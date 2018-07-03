using System.Linq;
using EmailService.Models;
using EmailService.Utils;

namespace EmailService.Dtos.Responses.Factories
{
	public static class EmailPreviewResponseFactory
	{
		public static EmailPreviewResponse Create(Email email)
		{
			string contentType = ContentTypeUtility.GetContentTypeString(email.ContentType);
			string[] receivers = email.To.Select(t => t.ToString()).ToArray();
				
			return new EmailPreviewResponse()
			{
				Preview = email.Content,
				ContentType = contentType,
				To = receivers,
				Subject = email.Subject
			};
		}
	}
}