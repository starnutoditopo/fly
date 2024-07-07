using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Fly.Services;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

/// <summary>
/// Converts <see cref="string[]" /> instances to <see cref="string" /> instances.
/// </summary>
//[ValueConversion(typeof(string[]), typeof(string))]
public class CsvStringArrayConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string[] items)
        {
            if (targetType == typeof(string))
            {
                return StringArraySerializationHelper.ToCsv(items);
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string csv)
        {
            if (targetType == typeof(string[]))
            {
                try
                {
                    return StringArraySerializationHelper.FromCsv(csv);
                }
                catch
                {
                    return new BindingNotification(new InvalidOperationException("Unable to deserialize the specified string."), BindingErrorType.Error);
                }
            }
        }

        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

