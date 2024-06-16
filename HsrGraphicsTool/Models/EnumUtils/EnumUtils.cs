using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace HsrGraphicsTool.Models.EnumUtils;

public static class EnumUtils
{
    public static object? GetEnumValue(Type targetType, string description)
    {
        // From the type, get all the enum Value/Description pair
        var enumValueDescriptions = GetAllValuesAndDescriptions(targetType);
        
        // Return the first enum value that matches the description
        return enumValueDescriptions
            .Where(x => (string)x.Description == description)
            .Select(x => x.Value)
            .FirstOrDefault();
    }
    
    /// <summary>
    /// Given an enum value, identify its description assigned using Description attribute
    /// </summary>
    /// <param name="value">Enum value</param>
    /// <returns>Description string of the enum</returns>
    public static string GetEnumDescription(Enum value)
    {
        string? description = null;

        // Get attributes associated with the enum value
        var attributes = value.GetType().GetField(value.ToString())?
            .GetCustomAttributes(typeof(DescriptionAttribute), false);
        if (attributes != null && attributes.Length != 0)
        {
            description = (attributes.First() as DescriptionAttribute)?.Description;
        }

        if (description != null)
        {
            return description;
        }

        // If no DescriptionAttribute is found, default to the enum value name
        var ti = CultureInfo.CurrentCulture.TextInfo;
        description = ti.ToTitleCase(ti.ToLower(value.ToString().Replace("_", " ")));

        return description;
    }

    public static IEnumerable<ValueDescription> GetAllValuesAndDescriptions(Type type)
    {
        var actualType = type;
        if (type.IsArray)
        {
            actualType = type.GetElementType();
        }
        if (actualType == null || !actualType.IsEnum)
        {
            throw new ArgumentException("Type must be an or enum array.");
        }

        return Enum.GetValues(actualType)
            .Cast<Enum>()
            .Select(value => new ValueDescription(value, GetEnumDescription(value)))
            .ToList();
    }
}