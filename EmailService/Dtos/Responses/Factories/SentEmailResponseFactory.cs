using EmailService.Models;
using EmailService.Utils;

namespace EmailService.Dtos.Responses.Factories
{
	public static class SentEmailResponseFactory
	{
		public static SentEmailResponse Create(LogEntry logEntry, EmailPreviewResponse preview, bool matchedReceiversToTest)
		{
			string[] logEntryTo = ArrayUtility.GetArrayFromCommaSeparatedString(logEntry.To);
			
			return new SentEmailResponse()
			{
				Id = logEntry.Id.ToString(),
				EmailServiceId = logEntry.EmailServiceId,
				
				Subject = preview.Subject,
				
				MatchedReceiversToTest = matchedReceiversToTest,
				NumberOfReceivers = logEntryTo.Length,
				
				ContentType = preview.ContentType,
				Message = preview.Preview
			};
		}
	}
}