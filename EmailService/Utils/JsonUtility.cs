using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EmailService.Utils
{
	public static class JsonUtility
	{
		public static void TraverseLeaves(JObject json, Func<string, string> callback)
		{
			TraverseLeavesRecursive(json, callback);
		}
		
		public static void TraverseLeaves(JArray json, Func<string, string> callback)
		{
			TraverseLeavesRecursive(json, callback);
		}

		private static void TraverseLeavesRecursive(JToken token, Func<string, string> callback)
		{
			bool isLeaf = IsObjectLeaf(token);

			if (isLeaf)
			{
				JProperty property = token as JProperty;
				property.Value = callback(property.Value.ToString());

				return;
			}

			bool isArray = token.Type == JTokenType.Array;

			if (isArray)
			{
				TraverseLeavesRecursiveArray(token, callback);
			}
			else
			{
				TraverseLeavesRecursiveObject(token, callback);	
			}
		}

		private static void TraverseLeavesRecursiveObject(JToken token, Func<string, string> callback)
		{
			foreach (JToken child in token)
			{
				TraverseLeavesRecursive(child, callback);
			}
		}

		private static void TraverseLeavesRecursiveArray(JToken token, Func<string, string> callback)
		{
			for (int i = 0; i < token.Children().Count(); i++)
			{
				JToken child = token.Children().ElementAt(i);
				bool isSingleChild = !child.Children().Any();

				if (isSingleChild)
				{
					string childString = child.ToString();
					string newChildString = callback(childString);
					JToken newChildToken = JToken.FromObject(newChildString);
					
					child.Replace(newChildToken);
				}
				else
				{
					TraverseLeavesRecursive(child, callback);
				}
			}
		}

		private static bool IsObjectLeaf(JToken token)
		{
			bool isProperty = token.Type == JTokenType.Property;

			if (!isProperty)
			{
				return false;
			}
			
			JProperty property = token as JProperty;
			return !property.Value.HasValues;
		}
	}
}