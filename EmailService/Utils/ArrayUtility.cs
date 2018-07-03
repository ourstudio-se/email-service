namespace EmailService.Utils
{
	public static class ArrayUtility
	{
		private const string COMMA_SEPARATION_DELIMITOR = ", ";
		
		public static string GetCommaSeparatedArray(string[] array)
		{
			return string.Join(COMMA_SEPARATION_DELIMITOR, array);
		}

		public static string[] GetArrayFromCommaSeparatedString(string commaSeparatedString)
		{
			return commaSeparatedString.Split(COMMA_SEPARATION_DELIMITOR);
		}
	}
}