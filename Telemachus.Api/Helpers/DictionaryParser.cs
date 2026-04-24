using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class DictionaryParser
    {
        private readonly Dictionary<string, object> _dict;

        public DictionaryParser(Dictionary<string, object> dict)
        {
            _dict = dict.ToDictionary(
                pair => NormalizeKey(pair.Key),
                pair => pair.Value
            );
        }

        private static string NormalizeKey(string key)
        {
            return key?.Trim().ToLower();
        }

        private object GetObjectItem(string key, string defaultKey = null)
        {
            key = NormalizeKey(key);
            defaultKey = NormalizeKey(defaultKey);

            if (key != null && _dict.TryGetValue(key, out var value))
            {
                return value;
            }
            else if (defaultKey != null && _dict.TryGetValue(defaultKey, out value))
            {
                return value;
            }
            return null;
        }

        public double? GetDouble(string key, string defaultKey = null)
        {
            object value = GetObjectItem(key, defaultKey);
            if (value == null) return null;

            if (value is long l)
            {
                return l;
            }
            else if (value is int i)
            {
                return i;
            }
            else if (value is short s)
            {
                return s;
            }
            else if (value is string str)
            {
                if (double.TryParse(str, out var result))
                {
                    return result;
                }
                return null;
            }
            else if (value is double d)
            {
                return d;
            }
            else if (value is float f)
            {
                return (double)f;
            }
            else if (value is decimal dec)
            {
                return (double)dec;
            }

            return value as double?;
        }

        public string GetString(string key, string defaultKey = null)
        {
            object value = GetObjectItem(key, defaultKey);
            return value?.ToString();
        }
    }

}
