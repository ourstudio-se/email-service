using System;
using EmailService.Models;

namespace EmailService.Utils
{
	public static class ContentTypeUtility
	{
		public static string GetContentTypeString(ContentType contentType)
		{
			switch (contentType)
			{
				case ContentType.TEXT_HTML:
					return "text/html";
				default:
					throw new ArgumentException("Not a valid content type value.");
			}
		}
	}
}