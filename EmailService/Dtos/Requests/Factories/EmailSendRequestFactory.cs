using EmailService.Models;

namespace EmailService.Dtos.Requests.Factories
{
	public static class EmailSendRequestFactory
	{
		public static bool IsValidEmailRequest(EmailSendRequest request)
		{
			bool hasTo = request.To != null && request.To.Length > 0;
			bool hasTemplate = !string.IsNullOrWhiteSpace(request.Template);
			bool hasContent = request.Content != null;

			return hasTo && hasTemplate && hasContent;
		}
	}
}