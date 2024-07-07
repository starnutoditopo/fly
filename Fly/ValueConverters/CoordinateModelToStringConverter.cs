using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Fly.Models;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

/// <summary>
/// Converts <see cref="CoordinateModel" /> instances to <see cref="string" /> instances.
/// </summary>
//[ValueConversion(typeof(CoordinateModel), typeof(string))]
public class CoordinateModelToStringConverter : MarkupExtension, IValueConverter
{
    private const char SEPARATOR = ';';
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CoordinateModel coordinate)
        {
            if (targetType == typeof(string))
            {
                return $"{coordinate.Latitude.ToString(CultureInfo.InvariantCulture)}{SEPARATOR}{coordinate.Longitude.ToString(CultureInfo.InvariantCulture)}";
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string s)
        {
            if (targetType == typeof(CoordinateModel))
            {
                try
                {
                    var values = s.Split(SEPARATOR);
                    if (values.Length == 2)
                    {
                        var latitudeStr = values[0];
                        var longitudeStr = values[1];
                        var latitude = double.Parse(latitudeStr, CultureInfo.InvariantCulture);
                        var longitude = double.Parse(longitudeStr, CultureInfo.InvariantCulture);
                        return new CoordinateModel()
                        {
                            Latitude = latitude,
                            Longitude = longitude
                        };
                    }
                }
                catch
                {
                    return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
                }
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
