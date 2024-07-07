using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Avalonia.Input.Platform;
using Fly.Models;
using Fly.Services;
using DialogHostAvalonia;
using ReactiveUI;
using Avalonia.Controls;

namespace Fly.ViewModels;

public partial class FlightPlansViewModel : ViewModelBase
{
    private readonly IClipboard _clipboard;
    private static readonly object _lockObject = new object();
    private readonly IMapper _mapper;
    private readonly ObservableCollection<PlaneBaseViewModel> _availablePlanes;
    private readonly ObservableCollection<RouteBaseViewModel> _availableRoutes;
    private readonly ISettingsService _settingsService;

    public FlightPlansViewModel(
        IClipboard clipboard,
        IMapper mapper,
        ObservableCollection<PlaneBaseViewModel> availablePlanes,
        ObservableCollection<RouteBaseViewModel> availableRoutes,
        ISettingsService settingsService
        )
    {
        _mapper = mapper;
        _availablePlanes = availablePlanes;
        _availableRoutes = availableRoutes;
        _settingsService = settingsService;
        _clipboard = clipboard;

        FlightPlans = new ObservableCollection<FlightPlanBaseViewModel>();
    }

    public ObservableCollection<FlightPlanBaseViewModel> FlightPlans { get; }

    private FlightPlanBaseViewModel? _selectedFlightPlan;
    public FlightPlanBaseViewModel? SelectedFlightPlan
    {
        get => _selectedFlightPlan;
        set
        {
            SetProperty(ref _selectedFlightPlan, value);
            this.RaisePropertyChanged(nameof(CanRemove));
            this.RaisePropertyChanged(nameof(CanEdit));
        }
    }

    public bool CanRemove => SelectedFlightPlan != null;

    public async Task Remove()
    {
        FlightPlanBaseViewModel? flightPlan = SelectedFlightPlan;
        if (flightPlan != null)
        {
            FlightPlans.Remove(flightPlan);
            SelectedFlightPlan = null;
        }
        await Task.CompletedTask;
    }

    public bool CanEdit => SelectedFlightPlan != null;

    public async Task Edit()
    {
        FlightPlanBaseViewModel? flightPlan = SelectedFlightPlan;
        if (flightPlan != null)
        {
            var cloneModel = _mapper.Map<FlightPlanModel>(flightPlan);
            var clone = _mapper.Map<FlightPlanBaseViewModel>(cloneModel);
            var viewModel = new EditFlightPlanViewModel(
                _clipboard,
                clone,
                _availablePlanes,
                _availableRoutes
            );
            if (flightPlan.Plane != null) {
                viewModel.FlightPlan.Plane = _availablePlanes.Single(p => p.Id == flightPlan.Plane.Id);
            }
            if (flightPlan.Route != null)
            {
                viewModel.FlightPlan.Route = _availableRoutes.Single(r => r.Id == flightPlan.Route.Id);
            }
            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditFlightPlan.Title") as string);
            dialogViewModel.ContentViewModel = viewModel;
            var ok = await DialogHost.Show(dialogViewModel);
            if (true.Equals(ok))
            {
                var model = _mapper.Map<FlightPlanModel>(clone);
                _mapper.Map(model, flightPlan);
                if (clone.Plane != null)
                {
                    flightPlan.Plane = _availablePlanes.Single(p => p.Id == clone.Plane.Id);
                }
                if (clone.Route != null)
                {
                    flightPlan.Route = _availableRoutes.Single(r => r.Id == clone.Route.Id);
                }
            }
        }
    }

    internal int GetIdForNewFlightPlan()
    {
        lock (_lockObject)
        {
            return FlightPlans.Max(x => x.Id) + 1;
        }
    }

    public async Task Add()
    {
        var residualAutonomy = _settingsService.GetDefaultResidualAutonomy();
        var timeToReachAlternateField = _settingsService.GetDefaultTimeToReachAlternateField();

        FlightPlanBaseViewModel flightPlanBaseViewModel = new FlightPlanViewModel()
        {
            Id = GetIdForNewFlightPlan(),
            ResidualAutonomy = residualAutonomy,
            TimeToReachAlternateField = timeToReachAlternateField
        };
        FlightPlans.Add(flightPlanBaseViewModel);
        await Task.CompletedTask;
    }
}
