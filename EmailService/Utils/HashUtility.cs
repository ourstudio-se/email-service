using System;
using System.Security.Cryptography;
using System.Text;

namespace EmailService.Utils
{
	public static class HashUtility
	{
		public static string GetStringHash(string value)
		{
			byte[] hash = GetBytesHash(value);
			return BitConverter.ToString(hash).Replace("-", "");
		}

		public static bool IsHashMatch(string hash, string value)
		{
			return GetStringHash(value).Equals(hash);
		}

		private static byte[] GetBytesHash(string value)
		{
			HashAlgorithm algorithm = SHA256.Create();
			return algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
		}
	}
}