using System.Collections.Generic;
using EmailService.Models;

namespace EmailService.Tools
{
	public class TemplateCache
	{
		private Dictionary<string, Template> Cache = new Dictionary<string, Template>();

		public Template GetTemplate(string name)
		{
			return Cache.GetValueOrDefault(name, null);
		}

		public void AddToCache(string name, Template template)
		{
			Cache.Add(name, template);
		}
	}
}