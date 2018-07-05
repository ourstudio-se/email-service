using System.IO;

namespace EmailService.Utils
{
	public static class ConfigurationUtility
	{
		public static T CreateInstance<T>(string path)
		{
			string fullPath = Path.GetFullPath(path);
			return FilesystemUtility.ReadJson<T>(fullPath);
		}
	}
}