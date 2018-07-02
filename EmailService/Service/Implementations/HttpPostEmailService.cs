using System;
using System.Net.Http;
using System.Threading.Tasks;
using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service.Implementations
{
	public class HttpPostEmailService : IEmailService
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		
		public async Task SendEmailAsync(EmailProperties emailProperties, IEmailServiceDefinition definition, Email email)
		{
			string url = definition.GetUrl();
			string body = definition.GetBody(emailProperties, email);

			string authKey = definition.GetAuthenticationHeaderKey();
			string authValue = definition.GetAuthenticationHeaderValue(emailProperties);
			
			StringContent stringContent = new StringContent(body);
			stringContent.Headers.Add(authKey, authValue);

			HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to send email.");
			}
		}
	}
}