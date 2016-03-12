using System.Collections.Generic;
using System.Collections.Specialized;

namespace JabbR.Infrastructure
{

    public static class EngagementExtensions
    {
        public static void CopyTo(this NameValueCollection source, IDictionary<string, string> destination)
        {
            foreach (string key in source.Keys)
            {
                if (destination.ContainsKey(key))
                {
                    destination[key] += ", " + source[key];
                }
                else
                {
                    destination.Add(key, source[key]);
                }
            }
        }
    }
}