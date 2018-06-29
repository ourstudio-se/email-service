using System.IO;
using System.Linq;
using EmailService.Configuration;
using EmailService.Models;
using EmailService.Tools;
using EmailService.Utils;

namespace EmailService.Service.Implementations
{
	public class FilesystemTemplatingService : ITemplatingService
	{
		private readonly TemplateCache _templateCache = new TemplateCache(); 
		
		public Template GetTemplate(EmailConfiguration emailConfiguration, string name)
		{
			TemplateDefinition definition = emailConfiguration.Templates.FirstOrDefault(d => d.Name.Equals(name));
			bool isInvalidDefinition = definition == null;

			if (isInvalidDefinition)
			{
				return null;
			}

			Template cachedTemplate = _templateCache.GetTemplate(name);
			bool hasCachedTemplate = cachedTemplate != null;

			if (hasCachedTemplate)
			{
				return cachedTemplate;
			}

			Template template = new Template()
			{
				Subject = definition.Subject,
				RawReact = GetRawReact(emailConfiguration.TemplatePath, definition.Name)
			};
			
			_templateCache.AddToCache(definition.Name, template);
			return template;
		}
		
		private string GetRawReact(string templatePath, string name)
		{
			string filePath = Path.Join(templatePath, name);
			string fullPath = Path.GetFullPath(filePath);

			return FilesystemUtility.ReadFile(fullPath);
		}
	}
}