using Newtonsoft.Json.Linq;

namespace EmailService.Utils
{
    public static class RestUtility
    {
        public static string[] GetArrayQueryParam(string queryParam)
        {
            bool isInvalidParam = string.IsNullOrWhiteSpace(queryParam);

            if (isInvalidParam)
            {
                return new string[0];
            }

            return JArray.Parse(queryParam).ToObject<string[]>();
        }
    }
}
