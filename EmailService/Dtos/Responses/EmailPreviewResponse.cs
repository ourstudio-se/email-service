namespace EmailService.Dtos.Responses
{
	public class EmailPreviewResponse
	{
		public string Subject { get; set; }
		public string[] To { get; set; }
		public string ContentType { get; set; }
		public string Preview { get; set; }
	}
}