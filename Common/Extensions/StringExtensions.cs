using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static System.String;

namespace AccountManager.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToUnderscoreCase(this string str)
        {
            return Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();
        }

        public static string ToSentenceCase(this string str)
        {
            return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1]));
        }

        public static string ToCamelCase(this string str)
        {
            if (!IsNullOrEmpty(str) && str.Length > 1)
            {
                return char.ToLowerInvariant(str[0]) + str.Substring(1);
            }

            return str;
        }

        public static string ToBase64(this string str)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(str);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return IsNullOrEmpty(str) ||
                   str.Trim().Length == 0
                ? true
                : false;
        }

        public static T[] DeserializeArray<T>(this string str)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(str)) return new T[0];
                str = str.Replace("{", "").Replace("}", "");

                return str.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => (T)Convert.ChangeType(x.Trim(), typeof(T))).ToArray();
            }
            catch (Exception e)
            {
                return Array.Empty<T>();
            }
        }
    }
}