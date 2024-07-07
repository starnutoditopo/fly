using System;

namespace Fly.ViewModels;
public class MessageDialogViewModel : ViewModelBase
{
    public enum MessageDialogMode
    {
        Information,
        Warning,
        Error
    }

    private readonly Exception? _exception;
    public MessageDialogViewModel(Exception exception)
    {
        _exception = exception;
        Message = _exception.Message;
        Mode = MessageDialogMode.Error;
    }

    public MessageDialogViewModel(string message)
    {
        Message = message;
        Mode = MessageDialogMode.Information;
    }

    public string Message { get; }

    public MessageDialogMode Mode { get; }
}
