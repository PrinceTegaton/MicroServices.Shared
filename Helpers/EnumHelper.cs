using System;
using System.ComponentModel;
using System.Linq;

namespace MicroServices
{
    public static class EnumHelper
    {
        public static T Parse<T>(this string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    return default;
                }

                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public static string GetName(this Enum value)
        {
            return Enum.GetName(value.GetType(), value);
        }

        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.GetName());
            return fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is not DescriptionAttribute descriptionAttribute ? value.GetName() : descriptionAttribute.Description;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.ToLower() == description.ToLower())
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.ToLower() == description.ToLower())
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }
            throw new ArgumentException("Enum value not found.", nameof(description));
        }
    }
}
