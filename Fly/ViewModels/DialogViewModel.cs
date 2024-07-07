namespace Fly.ViewModels;

public class DialogViewModel : ViewModelBase
{
    public DialogViewModel() : this(string.Empty)
    {
    }

    public DialogViewModel(string title)
    {
        Title = title;
    }
    public ViewModelBase? ContentViewModel {  get; set; }

    public string Title { get;  }
}