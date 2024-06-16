using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace HsrGraphicsTool.Models.EnumUtils;

/// <summary>
/// Converter class to convert an enum to a collection of its values and descriptions
/// </summary>
public class EnumCollectionConverter : MarkupExtension, IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return EnumUtils.GetAllValuesAndDescriptions(value!.GetType());
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

public class EnumValueConverter : MarkupExtension, IValueConverter
{
    
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new ValueDescription(value!, EnumUtils.GetEnumDescription((Enum)value!));
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null ? null : EnumUtils.GetEnumValue(targetType, (string)((ValueDescription) value).Description);
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}