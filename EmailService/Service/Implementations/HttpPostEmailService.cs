using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
			string url = emailProperties.EmailServiceUrl;
			string body = definition.GetBody(emailProperties, email);

			string authScheme = definition.GetAuthenticationHeaderScheme();
			string authValue = definition.GetAuthenticationHeaderValue(emailProperties);
			
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authScheme, authValue);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			
			StringContent stringContent = new StringContent(body, Encoding.UTF8, "application/json");
			
			HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to send email.");
			}
		}
	}
}