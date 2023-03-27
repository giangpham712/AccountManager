using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;

namespace AccountManager.Application.Mappings
{
    public abstract class ProfileBase : Profile
    {
        public static string Join<T>(IEnumerable<T> values, string separator)
        {
            return values == null ? null : string.Join(separator, values);
        }

        public static string[] Split(string valuesString, string separator)
        {
            if (string.IsNullOrEmpty(valuesString)) return new string[0];
            return valuesString
                .Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
        }

        public static string Serialize<T>(T t)
        {
            try
            {
                if (t == null) throw new ArgumentNullException();
                return JsonConvert.SerializeObject(t);
            }
            catch
            {
                return null;
            }
        }

        protected static string SerializeArray<TArray>(TArray t)
        {
            var serialized = Serialize(t);
            return serialized.Replace("[", "{").Replace("]", "}");
        }

        protected static string SerializeForPostgresTimestampArray(DateTimeOffset[] t)
        {
            return Serialize(t ?? new DateTimeOffset[0]).Replace("[", "{").Replace("]", "}");
        }

        protected DateTimeOffset[] DeserializeBackupTimes(string sTimes)
        {
            try
            {
                sTimes = sTimes.Replace("{", "[").Replace("}", "]");
                return JsonConvert.DeserializeObject<DateTimeOffset[]>(sTimes).ToArray();
            }
            catch (Exception e)
            {
                //
                return new DateTimeOffset[] { };
            }
        }

        protected static T Deserialize<T>(string tString, T tDefault = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tString)) return tDefault;
                return JsonConvert.DeserializeObject<T>(tString);
            }
            catch
            {
                return tDefault;
            }
        }
    }
}