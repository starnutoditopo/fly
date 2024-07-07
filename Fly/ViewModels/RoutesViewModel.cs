using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fly.Models;
using DialogHostAvalonia;
using ReactiveUI;
using Avalonia.Controls;

namespace Fly.ViewModels;

public class RoutesViewModel : ViewModelBase
{
    private static readonly object _lockObject = new object();
    private readonly IMapper _mapper;
    public ObservableCollection<RouteBaseViewModel> Routes { get; }

    public RoutesViewModel(
        IMapper mapper
        )
    {
        _mapper = mapper;
        Routes = new ObservableCollection<RouteBaseViewModel>();
    }

    private RouteBaseViewModel? _selectedRoute;
    public RouteBaseViewModel? SelectedRoute
    {
        get => _selectedRoute;
        set
        {
            SetProperty(ref _selectedRoute, value);
            this.RaisePropertyChanged(nameof(CanEdit));
            this.RaisePropertyChanged(nameof(CanRemove));
        }
    }

    public bool CanRemove => SelectedRoute != null;

    public async Task Remove()
    {
        RouteBaseViewModel? route = SelectedRoute;
        if (route != null)
        {
            Routes.Remove(route);
            SelectedRoute = null;
        }
        await Task.CompletedTask;
    }

    public bool CanEdit => SelectedRoute != null;

    
    public async Task Edit()
    {
        RouteBaseViewModel? route = SelectedRoute;
        if (route != null)
        {
            var cloneModel = _mapper.Map<RouteModel>(route);
            var clone = _mapper.Map<RouteBaseViewModel>(cloneModel);
            var viewModel = new EditRouteViewModel(clone);
            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditRoute.Title") as string);
            dialogViewModel.ContentViewModel = viewModel;
            var ok = await DialogHost.Show(dialogViewModel);
            if (true.Equals(ok))
            {
                var model = _mapper.Map<RouteModel>(clone);
                _mapper.Map(model, route);
            }
        }
    }

    internal int GetIdForNewRoute()
    {
        lock (_lockObject)
        {
            return Routes.Max(x => x.Id) + 1;
        }
    }

    public async Task Add()
    {
        RouteBaseViewModel routeBaseViewModel = new RouteViewModel();
        routeBaseViewModel.Id = GetIdForNewRoute();
        Routes.Add(routeBaseViewModel);
        await Task.CompletedTask;
    }
}
