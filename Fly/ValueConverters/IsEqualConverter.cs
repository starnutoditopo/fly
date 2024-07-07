using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Fly.Models;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

internal class IsEqualConverter : MarkupExtension, IValueConverter
{

    public IsEqualConverter() { }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (null == value)
        {
            return null == parameter;
        }
        return value.Equals(parameter);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (true.Equals(value))
        {
            return parameter;
        }
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


internal class IcaoClassToEmojiConverter : MarkupExtension, IValueConverter
{
    private const string EMOJI = "ⒶⒷⒸⒹⒺⒻⒼⒽⒾⒿⓀⓁⓂⓃⓄⓅⓆⓇⓈⓉⓊⓋⓌⓍⓎⓏ";
    //private const string EMOJI = "ABCDEFGHIJ";

    public IcaoClassToEmojiConverter() { }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is IcaoClass icaoClass)
        {
            int i = (int)icaoClass;
            if (i >= (int)IcaoClass.A && i <= (int)IcaoClass.G)
            {
                return new string(EMOJI[i], 1);
            };

            return string.Empty;
        }
        return new BindingNotification(new NotSupportedException($"Target type ({targetType}) is not supported."), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

}
