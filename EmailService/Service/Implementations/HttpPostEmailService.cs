using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EmailService.Configurations;
using EmailService.Models;

namespace EmailService.Service.Implementations
{
	public class HttpPostEmailService : IEmailService
	{
		private static readonly HttpClient _httpClient = new HttpClient();

		private readonly ServiceConfiguration _serviceConfiguration;
		private readonly IEmailServiceDefinition _emailServiceDefinition;
		private readonly EmailConfiguration _emailConfiguration;

		public HttpPostEmailService(ServiceConfiguration serviceConfiguration, IEmailServiceDefinition emailServiceDefinition,
			EmailConfiguration emailConfiguration)
		{
			_serviceConfiguration = serviceConfiguration;
			_emailServiceDefinition = emailServiceDefinition;
			_emailConfiguration = emailConfiguration;
		}
		
		public async Task<string> SendEmailAsync(Email email)
		{
			string url = _serviceConfiguration.EmailServiceUrl;
			string body = _emailServiceDefinition.GetBody(_emailConfiguration, email);

			string authScheme = _emailServiceDefinition.GetAuthenticationHeaderScheme();
			string authValue = _emailServiceDefinition.GetAuthenticationHeaderValue(_serviceConfiguration);
			
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