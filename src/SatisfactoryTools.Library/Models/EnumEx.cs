namespace SatisfactoryTools.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public static class EnumEx
    {
        public static T ParseFromDescription<T>(this string description)
            where T : struct
        {
            if (Enum.TryParse(description, true, out T value))
            {
                return value;
            }

            Type type = typeof(T);

            if (!type.IsEnum)
            {
                throw new InvalidOperationException();
            }

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (Attribute.GetCustomAttribute(
                    field,
                    typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            throw new ArgumentException($"{description} not found in {typeof(T).Name}.", nameof(description));

            // or return default(T);
        }
    }
}