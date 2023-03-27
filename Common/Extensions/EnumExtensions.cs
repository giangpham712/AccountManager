using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace AccountManager.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum enumValue)
        {
            var descriptionAttribute = enumValue.GetType().GetMember(enumValue.ToString()).First()
                .GetCustomAttribute<DescriptionAttribute>();

            return descriptionAttribute == null ? enumValue.ToString() : descriptionAttribute.Description;
        }
    }
}