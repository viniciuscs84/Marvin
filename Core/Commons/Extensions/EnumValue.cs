using System;
using System.Reflection;
using System.Resources;

namespace Marvin.Commons.Extensions
{
    /// <summary>
    /// Define extra enum values attributes
    /// </summary>
    /// <example>
    /// Example of an enum using EnumValue : Attribute
    /// <code>
    /// public enum DefaultRules
    /// {
    ///    [EnumValue(1, "View", typeof(GlobalResources.General))]
    ///    VIEW = 1,            
    ///    [EnumValue(2, "Record", typeof(GlobalResources.General))]
    ///    WRITE = 2,
    ///    [EnumValue(4, "Edit", typeof(GlobalResources.General))]
    ///    EDIT = 4,
    ///    [EnumValue(4, "Delete", typeof(GlobalResources.General))]
    ///    DELETE = 8
    /// }
    /// </code>
    /// </example>
    public class EnumValue : Attribute
    {
        /// <summary>
        /// Get value thats enum represents
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Get enum value label for GUI
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Get enum value type
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Get enum resource type for label globalization
        /// </summary>
        public Type ResourceType { get; private set; }

        /// <summary>
        /// Create a new instance of attribute
        /// </summary>
        /// <param name="value">Value thats enum represents</param>
        /// <param name="label">Enum value label for GUI</param>
        /// <param name="valueType">Enum value type</param>
        /// <param name="resourceType">Enum resource type for label globalization</param>
        public EnumValue(object value, string label = null, Type valueType = null, Type resourceType = null)
        {
            Value = value;
            Label = label;
            /* If valueType is undefined, get object value type */
            Type = valueType == null ? value.GetType() : valueType;
            ResourceType = resourceType;
        }

    }

    /// <summary>
    /// Enum extension that provides extra values attributes.
    /// </summary>
    /// <see cref="EnumValue"/>
    public static class EnumValueExtension
    {
        /// <summary>
        /// Get the actual value represented by enum
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Actual Enum value</returns>
        public static object GetEnumValue(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            EnumValue[] attribs = fieldInfo.GetCustomAttributes(typeof(EnumValue), false) as EnumValue[];
            return attribs.Length > 0 ? attribs[0].Value : value.ToString();
        }

        /// <summary>
        /// Get enum value label for GUI
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Value label for GUI</returns>
        public static string GetLabel(this Enum value)
        {
            Type type = value.GetType();
            string label = value.ToString();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            EnumValue[] attribs = fieldInfo.GetCustomAttributes(
                typeof(EnumValue), false) as EnumValue[];
            if (attribs.Length > 0 && !string.IsNullOrEmpty(attribs[0].Label))
            {
                label = attribs[0].Label;
                if (attribs[0].ResourceType != null)
                {
                    try
                    {
                        string transValue = new ResourceManager(attribs[0].ResourceType.FullName, attribs[0].ResourceType.Assembly)
                    .GetString(label);
                        if (!string.IsNullOrEmpty(transValue))
                            label = transValue;
                    }
                    catch (Exception ex)
                    {
                        Utilities.Logger.Error(ex);
                    }
                }
            }
            return label;
        }

        /// <summary>
        /// Get Actual value type represented by enum
        /// </summary>
        /// <param name="value">Enum value</param>
        /// <returns>Actual value type</returns>
        public static object GetEnumValueType(this Enum value)
        {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            EnumValue[] attribs = fieldInfo.GetCustomAttributes(typeof(EnumValue), false) as EnumValue[];
            return attribs.Length > 0 ? attribs[0].Type : typeof(string);
        }

        /// <summary>
        /// By the actual value returns the enum value
        /// </summary>
        /// <param name="enumType">Enum type</param>
        /// <param name="value">Actual value</param>
        /// <returns>Enum value</returns>
        /// <exception cref="NotSupportedException">enumType parameter is not a valid enum type</exception>
        public static Enum GetEnumByValue(Type enumType, object value)
        {
            if (!enumType.IsEnum)
                throw new NotSupportedException(enumType.FullName + " isn't a Enum Type");
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names)
            {
                if (GetEnumValue((Enum)Enum.Parse(enumType, name)).Equals(value))
                    return (Enum)Enum.Parse(enumType, name);
            }
            return null;
        }
    }
}
