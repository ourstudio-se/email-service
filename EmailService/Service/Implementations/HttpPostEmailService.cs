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

		private readonly ServiceProperties _serviceProperties;
		private readonly IEmailServiceDefinition _emailServiceDefinition;
		private readonly EmailProperties _emailProperties;

		public HttpPostEmailService(ServiceProperties serviceProperties, IEmailServiceDefinition emailServiceDefinition,
			EmailProperties emailProperties)
		{
			_serviceProperties = serviceProperties;
			_emailServiceDefinition = emailServiceDefinition;
			_emailProperties = emailProperties;
		}
		
		public async Task<string> SendEmailAsync(Email email)
		{
			string url = _serviceProperties.EmailServiceUrl;
			string body = _emailServiceDefinition.GetBody(_emailProperties, email);

			string authScheme = _emailServiceDefinition.GetAuthenticationHeaderScheme();
			string authValue = _emailServiceDefinition.GetAuthenticationHeaderValue(_serviceProperties);
			
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authScheme, authValue);
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			
			StringContent stringContent = new StringContent(body, Encoding.UTF8, "application/json");
			
			HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to send email.");
			}

			return _emailServiceDefinition.GetIdFromResponse(response);
		}
	}
}