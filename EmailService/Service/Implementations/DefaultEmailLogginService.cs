using System;
using System.Collections.Generic;
using System.Linq;
using EmailService.Models;
using EmailService.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EmailService.Service.Implementations
{
	public class DefaultEmailLogginService : IEmailLoggingService
	{
		public void Log(string emailServiceId, string[] receivers, string template, JObject personalContent, JObject content)
		{
			Guid id = Guid.NewGuid();
			DateTime timestamp = DateTime.Now;
			JObject obfuscatedPersonalContent = GetObfuscatedPersonalContent(personalContent);
			string[] hashedEmailReceivers = GetHashedEmailReceivers(receivers);

			string nonFormattedObfuscatedPersonalContent = obfuscatedPersonalContent.ToString(Formatting.None);
			string nonFormattedContent = content.ToString(Formatting.None);

			LogEntry logEntry = new LogEntry()
			{
				Id = id,
				EmailServiceId = emailServiceId,
				Timestamp = timestamp,
				To = hashedEmailReceivers,
				Template = template,
				PersonalContent = nonFormattedObfuscatedPersonalContent,
				Content = nonFormattedContent
			};

			int i = 0;
			//TODO save logEntry
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
	}
}