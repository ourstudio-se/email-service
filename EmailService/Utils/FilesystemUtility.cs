using System.IO;
using Newtonsoft.Json.Linq;

namespace EmailService.Utils
{
	public static class FilesystemUtility
	{
		public static T ReadJson<T>(string path)
		{
			string text = ReadFile(path);
			JObject json = JObject.Parse(text);

			return json.ToObject<T>();
		}

		public static string ReadFile(string path)
		{
			return File.ReadAllText(path);
		}
	}
}