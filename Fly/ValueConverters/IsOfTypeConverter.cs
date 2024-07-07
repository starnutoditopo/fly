using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

internal class IsOfTypeConverter : MarkupExtension, IValueConverter
{

    public IsOfTypeConverter() { }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value != null)
        {
            if (parameter is Type type)
            {
                if (type.IsAssignableTo(value.GetType()))
                {
                    return true;
                }
                return false;
            }
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }
        return new BindingNotification(new ArgumentNullException(nameof(value)), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

}
