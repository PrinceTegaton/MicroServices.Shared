using System;
using System.Collections.Generic;
using System.Reflection;

namespace MicroServices.Shared.Helpers
{
    public static class ObjectMapper
    {
        public static T Map<T>(this object source)
        {
            var newObj = Activator.CreateInstance<T>();
            var fields = newObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                var value = field.GetValue(source);
                field.SetValue(newObj, value);
            }

            return newObj;
        }

        public static List<T> Map<T>(this List<object> source)
        {
            var list = new List<T>();

            foreach (var item in source)
            {
                var newObj = Activator.CreateInstance<T>();
                var fields = newObj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (var field in fields)
                {
                    var value = field.GetValue(source);
                    field.SetValue(newObj, value);
                }

                list.Add(newObj);
            }

            return list;
        }
    }
}
