using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class EnumHelper
    {
        public static string GetDescriptionFromEnumValue(Enum value)
        {
            if (value == null)
            {
                return null;
            }

            DescriptionAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                .SingleOrDefault() as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static T GetValueFromDescription<T>(string description) where T : Enum
        {
            foreach (var field in typeof(T).GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default;
        }

        public static T GetAttribute<T>(Enum value) where T : Attribute
        {
            return value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(T), false)
                .SingleOrDefault() as T;
        }
    }
}
