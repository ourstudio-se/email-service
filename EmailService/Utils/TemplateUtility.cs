using System.Linq;
using EmailService.Configurations;
using EmailService.Models;

namespace EmailService.Utils
{
	public static class TemplateUtility
	{
		public static Template GetTemplateByName(EmailConfiguration emailConfiguration, string name)
		{
			return emailConfiguration.Templates.FirstOrDefault(d => d.Name.Equals(name));
		}
	}
}