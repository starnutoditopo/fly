namespace Fly.ViewModels;

public class EditRouteViewModel : ViewModelBase
{
    public EditRouteViewModel(RouteBaseViewModel route)
    {
        Route = route;
    }

    public RouteBaseViewModel Route { get; }
}
