
using Newtonsoft.Json.Linq;

namespace EmailService.Dtos.Requests.Factories
{
	public static class EmailSendRequestFactory
	{
		public static bool IsValidEmailRequest(EmailSendRequest request)
		{
			bool hasTo = request.To != null;
			bool hasTemplate = !string.IsNullOrWhiteSpace(request.Template);

			return hasTo && hasTemplate;
		}

		public static EmailSendRequest Convert(EmailSendRequestRaw request)
		{
			JObject content = new JObject();
			JObject personalContent = new JObject();

			bool hasContent = request.Content != null;
			bool hasPersonalContent = request.PersonalContent != null;

			if (hasContent)
			{
				content = JObject.Parse(request.Content);
			}

			if (hasPersonalContent)
			{
				personalContent = JObject.Parse(request.PersonalContent);
			}
			
			return new EmailSendRequest()
			{
				To = request.To,
				Template = request.Template,
				Content = content,
				PersonalContent = personalContent
			};
		}
	}
}