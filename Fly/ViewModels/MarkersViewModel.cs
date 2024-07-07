using Fly.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using DialogHostAvalonia;
using AutoMapper;
using Fly.Models;
using ReactiveUI;
using System;
using Avalonia.Controls;


namespace Fly.ViewModels;

public class MarkersViewModel : ViewModelBase
{
    private readonly IAirspaceInformationService _airspaceInformationService;
    private static readonly object _lockObject = new object();
    private readonly ISettingsService _settingsService;
    private readonly IReverseGeocodeService _reverseGeocodingService;
    private readonly IElevationService _elevationService;
    private readonly IMapper _mapper;
    public ObservableCollection<MarkerBaseViewModel> Markers { get; }

    public MarkersViewModel(
        IMapper mapper,
        ISettingsService settingsService,
        IReverseGeocodeService reverseGeocodingService,
        IElevationService elevationService,
        IAirspaceInformationService airspaceInformationService
        )
    {
        _airspaceInformationService = airspaceInformationService;
        _mapper = mapper;
        _settingsService = settingsService;
        _reverseGeocodingService = reverseGeocodingService;
        _elevationService = elevationService;
        Markers = new ObservableCollection<MarkerBaseViewModel>();
    }

    private MarkerBaseViewModel? _selectedMarker;
    public MarkerBaseViewModel? SelectedMarker
    {
        get => _selectedMarker;
        set
        {
            SetProperty(ref _selectedMarker, value);
            this.RaisePropertyChanged(nameof(CanEdit));
            this.RaisePropertyChanged(nameof(CanRemove));
        }
    }

    public bool CanRemove => SelectedMarker != null;

    public async Task Remove()
    {
        MarkerBaseViewModel? marker = SelectedMarker;
        if (marker != null)
        {
            Markers.Remove(marker);
            SelectedMarker = null;
        }
        await Task.CompletedTask;
    }

    public bool CanEdit => SelectedMarker != null;

    public async Task Edit()
    {
        MarkerBaseViewModel? marker = SelectedMarker;
        if (marker != null)
        {
            var fullCoordinateInformationModel = _mapper.Map<FullCoordinateInformationModel>(marker.Coordinate);
            var viewModel = new EditCoordinateViewModel(_mapper, _reverseGeocodingService, _elevationService, _settingsService, _airspaceInformationService, fullCoordinateInformationModel);
            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditCoordinate.Title") as string);
            dialogViewModel.ContentViewModel = viewModel;
            var ok = await DialogHost.Show(dialogViewModel);
            if (true.Equals(ok))
            {
                throw new NotImplementedException();
                //var model = _mapper.Map<MarkerModel>(clone);
                //_mapper.Map(model, marker);
            }
        }
    }

    internal int GetIdForNewMarker()
    {
        lock (_lockObject)
        {
            return Markers.Max(x => x.Id) + 1;
        }
    }

    public async Task Add()
    {
        MarkerBaseViewModel markerBaseViewModel = new MarkerViewModel(_settingsService, _reverseGeocodingService, _elevationService, _airspaceInformationService);
        markerBaseViewModel.Id = GetIdForNewMarker();
        Markers.Add(markerBaseViewModel);
        await Task.CompletedTask;
    }
}
