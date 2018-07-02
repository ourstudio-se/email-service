﻿using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Service
{
	public interface IEmailServiceDefinition
	{
		string GetUrl();
		string GetBody(EmailProperties emailProperties, Email email);

		string GetAuthenticationHeaderScheme();
		string GetAuthenticationHeaderValue(EmailProperties emailProperties);
	}
}