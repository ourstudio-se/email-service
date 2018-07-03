namespace EmailService.Dtos.Responses
{
	public class SentEmailResponse
	{
		public string Id { get; set; }
		public string EmailServiceId { get; set; }
		
		public string Subject { get; set; }
		
		public int NumberOfReceivers { get; set; }
		public bool MatchedReceiversToTest { get; set; }
		
		public string ContentType { get; set; }
		public string Message { get; set; }
	}
}