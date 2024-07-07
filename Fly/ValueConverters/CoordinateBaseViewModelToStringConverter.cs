using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Fly.ViewModels;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

/// <summary>
/// Converts <see cref="CoordinateBaseViewModel" /> instances to <see cref="string" /> instances.
/// </summary>
public class CoordinateBaseViewModelToStringConverter : MarkupExtension, IValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Backward conversion is not supported.");
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CoordinateBaseViewModel coordinate)
        {
            var result = $"{coordinate.Latitude.ToString("0.000", culture)} {Constants.COORDINATE_DECIMAL_FORMAT_SEPARATOR} {coordinate.Longitude.ToString("0.000", culture)}";
            return result;
        }
        return string.Empty;
    }
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

