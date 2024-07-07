using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Fly.Models;
using System;
using System.Globalization;

namespace Fly.ValueConverters;

public class VerticalLimitUnitToStringConverter : MarkupExtension, IValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Backward conversion is not supported.");
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VerticalLimitUnit verticalLimitUnit)
        {
            if (targetType == typeof(string))
            {
                switch (verticalLimitUnit)
                {
                    case VerticalLimitUnit.Feet:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitUnit.Feet") as string;
                    case VerticalLimitUnit.Meter:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitUnit.Meter") as string;
                    case VerticalLimitUnit.FlightLevel:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitUnit.FlightLevel") as string;
                }
            }
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

public class VerticalLimitReferenceDatumToStringConverter : MarkupExtension, IValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Backward conversion is not supported.");
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is VerticalLimitReferenceDatum verticalLimitReferenceDatum)
        {
            if (targetType == typeof(string))
            {
                switch (verticalLimitReferenceDatum)
                {
                    case VerticalLimitReferenceDatum.MSL:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitReferenceDatum.MSL") as string;
                    case VerticalLimitReferenceDatum.GND:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitReferenceDatum.GND") as string;
                    case VerticalLimitReferenceDatum.STD:
                        return Avalonia.Application.Current.FindResource("Text.VerticalLimitReferenceDatum.STD") as string;
                }
            }
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

