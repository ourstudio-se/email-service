using System.Linq;
using EmailService.Models;
using EmailService.Properties;

namespace EmailService.Utils
{
	public static class TemplateUtility
	{
		public static Template GetTemplateByName(EmailProperties properties, string name)
		{
			return properties.Templates.FirstOrDefault(d => d.Name.ToLower().Equals(name.ToLower()));
		}
	}
}