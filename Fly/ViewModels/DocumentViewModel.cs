using System.Diagnostics;
using Fly.Extensions;
using Fly.Models;
using Fly.Services;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Avalonia.Input.Platform;
using DialogHostAvalonia;
using Avalonia.Controls;

namespace Fly.ViewModels;

public class DocumentViewModel : ViewModelBase
{
    private readonly IMapper _mapper;
    private readonly IReverseGeocodeService _reverseGeocodeService;
    private readonly IElevationService _elevationService;
    private readonly ISettingsService _settingsService;
    private readonly IClipboard _clipboard;
    private readonly IAirspaceInformationService _airspaceInformationService;

    public DocumentViewModel(
        IClipboard clipboard,
        LayersViewModel layersViewModel,
        MarkersViewModel markersViewModel,
        RoutesViewModel routesViewModel,
        PlanesViewModel planesViewModel,
        IMapper mapper,
        ISettingsService settingsService,
        IReverseGeocodeService reverseGeocodeService,
        IElevationService elevationService,
        IAirspaceInformationService airspaceInformationService
    )
    {
        _reverseGeocodeService = reverseGeocodeService;
        _elevationService = elevationService;
        _settingsService = settingsService;
        _mapper = mapper;
        _clipboard = clipboard;
        _airspaceInformationService = airspaceInformationService;

        Tools = new List<MapEditTools> {
            MapEditTools.None,
            MapEditTools.AddMarker,
            MapEditTools.AddRoute,
            MapEditTools.Modify,
            MapEditTools.Delete
        };
        SelectedTool = MapEditTools.None;
        //ZoomLevel = 13;
        Rotation = 0;
        Layers = layersViewModel;
        Markers = markersViewModel;
        Routes = routesViewModel;
        Planes = planesViewModel;

        FlightPlansViewModel flightPlansViewModel = new FlightPlansViewModel(_clipboard, _mapper, Planes.Planes, Routes.Routes, _settingsService);
        FlightPlans = flightPlansViewModel;
    }

    #region Tool
    public IEnumerable<MapEditTools> Tools { get; }

    private MapEditTools _selectedTool;
    public MapEditTools SelectedTool
    {
        get => _selectedTool;
        set => SetProperty(ref _selectedTool, value);

    }
    #endregion

    #region Rotation
    private double _rotation;
    public double Rotation
    {
        get => _rotation;
        set => SetProperty(ref _rotation, value);
    }

    public double MinRotation { get; set; } = 0.0;
    public double MaxRotation { get; set; } = 360.0;

    #endregion

    public LayersViewModel Layers { get; }

    //public async Task GoHome(object view)
    //{
    //    throw new NotImplementedException();
    //}

    public async Task InfoRequest(CoordinateModel coordinate)
    {
        FullCoordinateInformationModel fullCoordinateInformationModel = new FullCoordinateInformationModel() { 
            Coordinate = coordinate,
        };
        EditCoordinateViewModel coordinateInformationViewModel = new EditCoordinateViewModel(_mapper, _reverseGeocodeService,_elevationService, _settingsService, _airspaceInformationService, fullCoordinateInformationModel);

        var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditCoordinate.Title") as string);
        dialogViewModel.ContentViewModel = coordinateInformationViewModel;
        await DialogHost.Show(dialogViewModel);
    }

    #region Markers

    public MarkersViewModel Markers { get; }

    public async Task AddingNewMarker(CoordinateModel coordinate)
    {
        MarkerViewModel markerViewModel = new MarkerViewModel(_settingsService, _reverseGeocodeService, _elevationService, _airspaceInformationService);
        markerViewModel.Id = Markers.GetIdForNewMarker();
        markerViewModel.Coordinate.Latitude = coordinate.Latitude;
        markerViewModel.Coordinate.Longitude = coordinate.Longitude;

        Markers.Markers.Add(markerViewModel);
        await Task.CompletedTask;
    }

    public async Task StartMovingMarker((MarkerViewModel, CoordinateModel) args)
    {
        Trace.WriteLine("StartMovingMarker");
        MarkerViewModel viewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;

        Markers.SelectedMarker = viewModel;
        if (viewModel != null)
        {
            viewModel.CoordinateAtDraggingStart = viewModel.Coordinate.ToCoordinateModel();
            viewModel.MouseStartDraggingCoordinate = coordinate;
        }
        await Task.CompletedTask;
    }

    public async Task MovingMarker((MarkerBaseViewModel, CoordinateModel) args)
    {
        Trace.WriteLine("MovingMarker");
        MarkerBaseViewModel markerViewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;

        Markers.SelectedMarker = markerViewModel;
        if (markerViewModel != null)
        {
            var coordinateViewModel = markerViewModel.Coordinate;
            coordinateViewModel.Latitude = coordinate.Latitude;
            coordinateViewModel.Longitude = coordinate.Longitude;
        }
        await Task.CompletedTask;
    }

    public async Task EndMovingMarker(MarkerViewModel markerViewModel)
    {
        Trace.WriteLine("EndMovingMarker");
        markerViewModel.CoordinateAtDraggingStart = null;
        markerViewModel.MouseStartDraggingCoordinate = null;
        await Task.CompletedTask;
    }

    public async Task DeleteMarker(MarkerBaseViewModel viewModel)
    {
        Trace.WriteLine("DeleteMarker");
        Markers.Markers.Remove(viewModel);
        await Task.CompletedTask;
    }

    #endregion
    #region Routes
    public RoutesViewModel Routes { get; }

    #region Create new Route
    public async Task StartDrawingNewRoute(CoordinateModel coordinate)
    {
        Trace.WriteLine("StartDrawingNewRoute");

        RouteViewModel routeViewModel = new RouteViewModel();
        routeViewModel.Id = Routes.GetIdForNewRoute();

        var coordinate0 = new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService)
        {
            Latitude = coordinate.Latitude,
            Longitude = coordinate.Longitude
        };

        var coordinate1 = new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService)
        {
            Latitude = coordinate.Latitude,
            Longitude = coordinate.Longitude
        };

        routeViewModel.Coordinates.Add(coordinate0);
        routeViewModel.Coordinates.Add(coordinate1);

        routeViewModel.EditingPointIndex = routeViewModel.Coordinates.Count - 1;

        Routes.Routes.Add(routeViewModel);
        Routes.SelectedRoute = routeViewModel;
        await Task.CompletedTask;
    }
    public async Task HoveringNextPointOfRoute((RouteViewModel, CoordinateModel) args)
    {
        Trace.WriteLine("HoveringNextPointOfRoute");
        RouteViewModel viewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;
        if (viewModel != null)
        {
            var coordinateViewModel = viewModel.Coordinates[viewModel.EditingPointIndex.Value];
            coordinateViewModel.Latitude = coordinate.Latitude;
            coordinateViewModel.Longitude = coordinate.Longitude;
        }
        await Task.CompletedTask;
    }
    public async Task AddingNextWaypointToNewRoute((RouteViewModel, CoordinateModel) args)
    {
        Trace.WriteLine("AddingNextWaypointToNewRoute");
        RouteViewModel viewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;
        if (viewModel != null)
        {
            if (viewModel.EditingPointIndex == null)
            {
                Debugger.Break();
            }

            //viewModel.Coordinates[viewModel.EditingPointIndex.Value] = coordinate;
            //viewModel.Coordinates.Add(coordinate);


            var coordinateViewModel = viewModel.Coordinates[viewModel.EditingPointIndex.Value];
            coordinateViewModel.Latitude = coordinate.Latitude;
            coordinateViewModel.Longitude = coordinate.Longitude;
            viewModel.Coordinates.Add(new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService)
            {
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude
            });

            viewModel.EditingPointIndex = viewModel.Coordinates.Count - 1;
        }
        await Task.CompletedTask;
    }
    public async Task EndDrawingNewRoute(RouteViewModel? viewModel)
    {
        Trace.WriteLine("EndDrawingNewRoute");
        if (viewModel != null)
        {
            viewModel.Coordinates.RemoveAt(viewModel.EditingPointIndex.Value);
            viewModel.EditingPointIndex = null;
        }
        await Task.CompletedTask;
    }
    #endregion

    public async Task WaypointInsertedIntoRoute((RouteBaseViewModel, CoordinateModel, int) args)
    {
        Trace.WriteLine("WaypointInsertedIntoRoute");
        RouteBaseViewModel viewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;
        int index = args.Item3;

        Routes.SelectedRoute = viewModel;
        if (viewModel != null)
        {
            viewModel.Coordinates.Insert(index, new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService)
            {
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude
            });
        }
        await Task.CompletedTask;
    }
    public async Task StartModifyingRoute((RouteViewModel, int, CoordinateModel) args)
    {
        Trace.WriteLine("StartModifyingRoute");
        RouteViewModel viewModel = args.Item1;
        int index = args.Item2;
        CoordinateModel mouseCoordinate = args.Item3;

        Routes.SelectedRoute = viewModel;
        if (viewModel != null)
        {
            viewModel.CoordinatesAtDraggingStart = viewModel
                .Coordinates
                .Select(c => c.ToCoordinateModel())
                .ToArray();
            viewModel.MouseStartDraggingCoordinate = mouseCoordinate;
            viewModel.EditingPointIndex = index;
        }
        await Task.CompletedTask;
    }
    public async Task ModifyingRoute((RouteBaseViewModel, CoordinateModel, int) args)
    {
        Trace.WriteLine("ModifyingRoute");
        RouteBaseViewModel viewModel = args.Item1;
        CoordinateModel coordinate = args.Item2;
        int index = args.Item3;

        Routes.SelectedRoute = viewModel;
        if (viewModel != null)
        {
            var coordinateViewModel = viewModel.Coordinates[index];
            coordinateViewModel.Latitude = coordinate.Latitude;
            coordinateViewModel.Longitude = coordinate.Longitude;
        }
        await Task.CompletedTask;
    }
    public async Task EndModifyingRoute(RouteViewModel viewModel)
    {
        Trace.WriteLine("EndModifyingRoute");
        viewModel.CoordinatesAtDraggingStart = null;
        viewModel.EditingPointIndex = null;
        viewModel.MouseStartDraggingCoordinate = null;
        await Task.CompletedTask;
    }
    public async Task DeleteRouteWaypoint((RouteBaseViewModel, int) args)
    {
        Trace.WriteLine("DeleteRouteWaypoint");
        RouteBaseViewModel viewModel = args.Item1;
        int index = args.Item2;

        viewModel.Coordinates.RemoveAt(index);
        await Task.CompletedTask;
    }

    public async Task DeleteRoute(RouteBaseViewModel viewModel)
    {
        Trace.WriteLine("DeleteRoute");
        Routes.Routes.Remove(viewModel);
        await Task.CompletedTask;
    }

    #endregion
    #region Planes
    public PlanesViewModel Planes { get; }

    #endregion

    #region FlightPlans
    public FlightPlansViewModel FlightPlans { get; }

    #endregion

    //public Task HandleAsync(CopyMessage message, CancellationToken cancellationToken)
    //{
    //    GpsData data = new GpsData();
    //    foreach (var markerViewModel in Markers.Markers)
    //    {
    //        var coordinate = markerViewModel.Coordinate.ToGeoCoordinate();
    //        Geo.Geometries.Point point = new Geo.Geometries.Point(coordinate);
    //        Waypoint waypoint = new Waypoint(point, markerViewModel.DisplayName, null, null);
    //        data.Waypoints.Add(waypoint);
    //    }
    //    foreach (var routeViewModel in Routes.Routes)
    //    {
    //        Route route = new Route();
    //        route.Metadata.Add("name", routeViewModel.DisplayName);
    //        foreach (var coordinateViewModel in routeViewModel.Coordinates)
    //        {
    //            var coordinate = coordinateViewModel.ToGeoCoordinate();
    //            Geo.Geometries.Point point = new Geo.Geometries.Point(coordinate);
    //            Waypoint waypoint = new Waypoint(point, null, null, null);
    //            route.Waypoints.Add(waypoint);
    //        }
    //        data.Routes.Add(route);
    //    }
    //    string gpx = data.ToGpx();
    //    Clipboard.SetText(gpx);
    //    return Task.CompletedTask;
    //}
}
