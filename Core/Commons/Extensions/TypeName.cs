using System;

namespace Marvin.Commons.Extensions
{
    /// <summary>
    /// Extension to get type name, treat generic type names
    /// </summary>
    public static class TypeNameExtension
    {
        /// <summary>
        /// Get the type name with treating for generic type name and removing "`" chars
        /// </summary>
        /// <param name="type">A type object</param>
        /// <returns>Name string of the type</returns>
        public static string GetTypeName(this Type type)
        {
            if (type.IsGenericParameter || !type.IsGenericType)
                return type.Name;
            var builder = new System.Text.StringBuilder();
            var name = type.Name;
            var index = name.IndexOf("`");
            builder.AppendFormat("{0}", name.Substring(0, index));
            return builder.ToString();
        }
    }
}
