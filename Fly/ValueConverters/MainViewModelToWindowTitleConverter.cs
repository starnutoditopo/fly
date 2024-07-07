using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;
using System.Text;

namespace Fly.ValueConverters;

public class MainViewModelToWindowTitleConverter : MarkupExtension, IValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Backward conversion is not supported.");
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        StringBuilder stringBuilder = new StringBuilder(Constants.PRODUCT_NAME);
        if (value is string documentName)
        {
            if (string.IsNullOrWhiteSpace(documentName))
            {
                stringBuilder.Append($" - {Avalonia.Application.Current.FindResource("Text.MainViewModel.NoDocument") as string}");
            }
            else { 
                stringBuilder.Append($" - {documentName}");
            }
        }
        return stringBuilder.ToString();
    }
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

