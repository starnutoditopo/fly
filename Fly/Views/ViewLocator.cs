using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Fly.ViewModels;
using System;

namespace Fly.Views
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data == null)
            {
                return new TextBlock { Text = "<Null>" };
            }
            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                var result = Activator.CreateInstance(type) as Control;
                if (result == null)
                {
                    return new TextBlock { Text = $"Unable to create an instance of type {type}" };
                }
                return result;
            }
            else
            {
                return new TextBlock { Text = $"View not found for {name}" };
            }
        }

        public bool Match(object? data)
        {
            return data is ViewModelBase;
        }
    }
}
