using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Globalization;
using static Fly.ViewModels.MessageDialogViewModel;

namespace Fly.ValueConverters;

internal class MessageDialogModeToIconConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageDialogMode messageDialogMode)
        {
            switch (messageDialogMode)
            {
                case MessageDialogMode.Information:
                    return Avalonia.Application.Current.FindResource("info_regular") as StreamGeometry;
                case MessageDialogMode.Warning:
                    return Avalonia.Application.Current.FindResource("warning_regular") as StreamGeometry;                    
                case MessageDialogMode.Error:
                    return Avalonia.Application.Current.FindResource("error_circle_regular") as StreamGeometry;
            }
        }
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}


internal class MessageDialogModeToIconColorConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageDialogMode messageDialogMode)
        {
            switch (messageDialogMode)
            {
                case MessageDialogMode.Information:
                    return Brushes.DarkBlue;
                case MessageDialogMode.Warning:
                    return Brushes.Yellow;
                case MessageDialogMode.Error:
                    return Brushes.DarkRed;
            }
        }
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
