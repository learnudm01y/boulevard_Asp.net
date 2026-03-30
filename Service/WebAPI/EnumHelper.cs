using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Boulevard.Service.WebAPI
{
    public class EnumHelper
    {
        public static List<object> GetEnumList<T>(string lang = "en") where T : struct, System.Enum
        {
            return System.Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new
                {
                    Name = GetDisplayName(e, lang),
                    Value = Convert.ToInt32(e)
                })
                .ToList<object>();
        }

        public static string GetDisplayName<T>(T value, string lang) where T : struct, System.Enum
        {
            var field = typeof(T).GetField(value.ToString());
            var displayAttr = field.GetCustomAttribute<DisplayAttribute>();

            if (displayAttr != null)
            {
                return lang.Equals("ar", StringComparison.OrdinalIgnoreCase)
                    ? displayAttr.Description // Arabic name
                    : displayAttr.Name;       // English name
            }

            return value.ToString();
        }

        public static int GetEnumValueByName<T>(string name, string lang = "en") where T : struct, System.Enum
        {
            foreach (var e in System.Enum.GetValues(typeof(T)).Cast<T>())
            {
                var field = typeof(T).GetField(e.ToString());
                var displayAttr = field.GetCustomAttribute<DisplayAttribute>();

                if (displayAttr != null)
                {
                    var match = lang.Equals("ar", StringComparison.OrdinalIgnoreCase)
                        ? displayAttr.Description
                        : displayAttr.Name;

                    if (string.Equals(match, name, StringComparison.OrdinalIgnoreCase))
                        return Convert.ToInt32(e);
                }
            }

            // If no match found
            return 0;
        }
    }
}