namespace EmailService.Dtos.Requests
{
	public class EmailSendRequestRaw
	{
		public string[] To { get; set; }
		public string Template { get; set; }
		
		public string PersonalContent { get; set; }
		public string Content { get; set; }
	}
}