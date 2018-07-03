using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using EmailService.Database;
using EmailService.Models;
using EmailService.Properties;
using EmailService.Utils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmailService.Service.Implementations
{
	public class DefaultEmailLogginService : IEmailLoggingService
	{
		private static readonly HttpClient _httpClient = new HttpClient();
		
		private readonly DataContext _dataContext;
		private readonly ServiceProperties _serviceProperties;

		public DefaultEmailLogginService(DataContext dataContext, ServiceProperties serviceProperties)
		{
			_dataContext = dataContext;
			_serviceProperties = serviceProperties;
		}
		
		public async Task LogAsync(string emailServiceId, string[] receivers, string template,
			JObject personalContent, JObject content)
		{
			Guid id = Guid.NewGuid();
			DateTime timestamp = DateTime.Now;
			JObject obfuscatedPersonalContent = GetObfuscatedPersonalContent(personalContent);
			string[] hashedEmailReceivers = GetHashedEmailReceivers(receivers);

			string singleStringEmailReceivers = string.Join(", ", hashedEmailReceivers);

			string nonFormattedObfuscatedPersonalContent = obfuscatedPersonalContent.ToString(Formatting.None);
			string nonFormattedContent = content.ToString(Formatting.None);

			LogEntry logEntry = new LogEntry()
			{
				Id = id,
				EmailServiceId = emailServiceId,
				Timestamp = timestamp,
				To = singleStringEmailReceivers,
				Template = template,
				PersonalContent = nonFormattedObfuscatedPersonalContent,
				Content = nonFormattedContent
			};

			bool isApiLogging = _serviceProperties.SelectedLoggingType.Equals(ServiceProperties.LoggingType.API);
			bool isDatabaseLogging = _serviceProperties.SelectedLoggingType.Equals(ServiceProperties.LoggingType.DATABASE);

			if (isApiLogging)
			{
				await LogToApiAsync(logEntry);
			}
			else if (isDatabaseLogging)
			{
				await LogToDatabaseAsync(logEntry);
			}
		}

		private JObject GetObfuscatedPersonalContent(JObject personalContent)
		{
			JsonUtility.TraverseLeaves(personalContent, ObfuscateLeaf);

			return personalContent;
		}

		private string ObfuscateLeaf(string value)
		{
			bool isNull = value == null;

			if (isNull)
			{
				return "null";
			}
			else
			{
				int length = value.Length;
				return $"string({length})";	
			}
		}

		private string[] GetHashedEmailReceivers(string[] receivers)
		{
			return receivers.Select(HashUtility.GetStringHash).ToArray();
		}

		private async Task LogToApiAsync(LogEntry logEntry)
		{
			string url = _serviceProperties.LoggingApiUrl;
			string body = ConvertToJson(logEntry);
			
			_httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			StringContent stringContent = new StringContent(body, Encoding.UTF8, "application/json");
			
			HttpResponseMessage response = await _httpClient.PostAsync(url, stringContent);

			if (!response.IsSuccessStatusCode)
			{
				throw new Exception("Failed to log email to api.");
			}
		}

		private async Task LogToDatabaseAsync(LogEntry logEntry)
		{
			_dataContext.Logs.Add(logEntry);
			await _dataContext.SaveChangesAsync();
		}

		private string ConvertToJson(LogEntry logEntry)
		{
			return JObject.FromObject(logEntry).ToString(Formatting.None);
		}
	}
}