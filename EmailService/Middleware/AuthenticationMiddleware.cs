using System.Threading.Tasks;
using EmailService.Configurations;
using Microsoft.AspNetCore.Http;

namespace EmailService.Middleware
{
	public class AuthenticationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ServiceConfiguration _serviceConfiguration;
		
		public AuthenticationMiddleware(ServiceConfiguration serviceConfiguration, RequestDelegate next)
		{
			_serviceConfiguration = serviceConfiguration;
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			string authenticationHeader = context.Request.Headers["Authorization"];
			
			bool isValidAuthentication = IsValidAuthentication(authenticationHeader);

			if (isValidAuthentication)
			{
				await _next.Invoke(context);
			}
			else
			{
				FailContext(context);
			}
		}

		private bool IsValidAuthentication(string authenticationHeader)
		{
			bool hasNoServiceApiKey = string.IsNullOrWhiteSpace(_serviceConfiguration.ServiceApiKey);

			if (hasNoServiceApiKey)
			{
				return true;
			}

			return _serviceConfiguration.ServiceApiKey.Equals(authenticationHeader);
		}

		private void FailContext(HttpContext context)
		{
			context.Response.StatusCode = 401;
		}
	}
}