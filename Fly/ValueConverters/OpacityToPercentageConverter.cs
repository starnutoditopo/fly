using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

public class OpacityToPercentageConverter : MarkupExtension, IValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)value / 100.0;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (int)((double)value * 100.0);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
