using System.IO;
using Newtonsoft.Json.Linq;

namespace EmailService.Utils
{
	public static class FilesystemUtility
	{
		public static T ReadJson<T>(string path)
		{
			string text = File.ReadAllText(path);
			JObject json = JObject.Parse(text);

			return json.ToObject<T>();
		}
	}
}