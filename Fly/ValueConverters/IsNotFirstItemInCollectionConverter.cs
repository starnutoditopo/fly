using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Fly.ValueConverters;

internal class IsNotFirstItemInCollectionConverter : MarkupExtension, IMultiValueConverter
{
    public IsNotFirstItemInCollectionConverter() { }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values != null)
        {
            if (values.Count == 2)
            {
                if (values[1] is IEnumerable items)
                {
                    if (targetType == typeof(bool))
                    {
                        if (items.Cast<object>().FirstOrDefault() == values[0])
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
        }
        return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;

}
