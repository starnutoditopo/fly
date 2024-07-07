using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Nts.Editing;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using Mapsui.UI;
using Mapsui.Widgets;
using NetTopologySuite.Geometries;
using Fly.Extensions;
using Fly.Features;
using Fly.Models;
using Fly.ViewModels;
using Point = NetTopologySuite.Geometries.Point;
using Mapsui.Manipulations;
using System.Linq;
using System;
using System.Collections.Generic;


namespace Fly.Widgets;

public class FlyEditingWidget : BaseWidget
{
    private enum MouseState
    {
        SingleClick,
        Dragging, // moving with mouse down
        Moving, // moving with mouse up
        DoubleClick
    }

    public IMapControl MapControl { get; }

    public WidgetEditMode EditMode { get; set; }

    public bool SelectMode { get; set; }

    public WritableLayer? Layer { get; set; }
    public FlyEditingWidget(IMapControl mapControl)
    {
        MapControl = mapControl;
        EditMode = WidgetEditMode.None;
    }

    public override bool OnTapped(Navigator navigator, WidgetEventArgs args)
    {
        if (args.TapType == TapType.Double)
        {
            MapControl.Map.Navigator.PanLock = Manipulate(MouseState.DoubleClick, args.Position, MapControl);
            return true;
        }
        else if (args.TapType == TapType.Single)
        {
            MapControl.Map.Navigator.PanLock = Manipulate(MouseState.SingleClick, args.Position, MapControl);
            return true;
        }

        return false;
    }
    public override bool OnPointerReleased(Navigator navigator, WidgetEventArgs args)
    {
        if (!args.LeftButton)
        {
            return false;
        }

        if (MapControl.Map != null)
        {
            if (EditMode == WidgetEditMode.Modify)
                StopDragging();

            MapControl.Map.Navigator.PanLock = false;
        }
        return false;
    }

    public override bool OnPointerPressed(Navigator navigator, WidgetEventArgs args)
    {
        if (!args.LeftButton)
        {
            return false;
        }

        var map = MapControl.Map;
        if (map != null)
        {
            // Take into account VertexRadius in feature select, because the objective
            // is to select the vertex.
            var mapInfo = MapControl.GetMapInfo(args.Position, VertexRadius);
            if (EditMode == WidgetEditMode.Modify && mapInfo?.Feature != null)
            {
                map.Navigator.PanLock = StartDragging(mapInfo, VertexRadius);
            }
            else
            {
                map.Navigator.PanLock = false;
            }
        }
        return false;
    }

    public override bool OnPointerMoved(Navigator navigator, WidgetEventArgs args)
    {
        if (args.LeftButton)
        {
            Manipulate(MouseState.Dragging, args.Position, MapControl);
        }
        else
        {
            Manipulate(MouseState.Moving, args.Position, MapControl);
        }

        return false;
    }

    #region manipulation

    private bool Manipulate(
        MouseState mouseState,
        ScreenPosition screenPosition,
        IMapControl mapControl
        )
    {
        switch (mouseState)
        {
            case MouseState.SingleClick:
                {
                    var mapInfo = mapControl.GetMapInfo(screenPosition, VertexRadius);
                    if (EditMode == WidgetEditMode.None)
                    {
                        CoordinateModel coordinate = ToCoordinateModel(mapInfo.WorldPosition);
                        OnInfoRequest(coordinate);
                    }
                    else if (EditMode == WidgetEditMode.Delete)
                    {
                        if (mapInfo.Feature == null)
                        {
                            return false;
                        }
                        if (mapInfo.Feature is MarkerFeature markerFeature)
                        {
                            OnDeleteMarker(markerFeature.ViewModel);
                            return false;
                        }
                        if (mapInfo.Feature is RouteFeature routeFeature)
                        {
                            OnDeleteRoute(routeFeature.ViewModel);
                            return false;
                        }
                        if (mapInfo.Feature is AnchorFeature anchorFeature)
                        {
                            var parentFeature = anchorFeature.ParentFeature as RouteFeature;
                            OnDeleteRouteWaypoint(parentFeature.ViewModel, anchorFeature.Index);
                            if (!parentFeature.ViewModel.Coordinates.Any())
                            {
                                OnDeleteRoute(parentFeature.ViewModel);
                            }
                            return false;
                        }
                        throw new NotImplementedException($"Cannot delete feature '{mapInfo.Feature}'.");
                    }
                    else if (EditMode == WidgetEditMode.Modify)
                    {
                        if (mapInfo?.WorldPosition is null) return false;

                        if (mapInfo.Feature is RouteFeature routeFeature)
                        {
                            if (routeFeature.Geometry is LineString lineString)
                            {
                                var vertices = lineString.MainCoordinates();
                                if (EditHelper.ShouldInsert(mapInfo.WorldPosition, mapInfo.Resolution, vertices, VertexRadius, out var index))
                                {
                                    Coordinate worldPosition = mapInfo.WorldPosition.ToCoordinate();
                                    InsertWaypointIntoRoute(routeFeature, worldPosition, index + 1);
                                }
                            }
                            else if (routeFeature.Geometry is Point point)
                            {
                                Coordinate worldPosition = mapInfo.WorldPosition.ToCoordinate();
                                InsertWaypointIntoRoute(routeFeature, worldPosition, 1);
                            }
                        }
                        return false;
                    }
                    else if (EditMode == WidgetEditMode.AddMarker)
                    {
                        CoordinateModel coordinate = ToCoordinateModel(mapInfo.WorldPosition);
                        OnNewMarkerAdded(coordinate);
                    }
                    else if (EditMode == WidgetEditMode.AddRoute)
                    {
                        CoordinateModel coordinate = ToCoordinateModel(mapInfo.WorldPosition);
                        OnStartDrawingNewRoute(coordinate);

                        EditMode = WidgetEditMode.DrawingRoute;
                    }
                    else if (EditMode == WidgetEditMode.DrawingRoute)
                    {
                        if (_myAddInfo.EditingIndex == null) return false;
                        if (_myAddInfo.Coordinates == null) return false;

                        CoordinateModel coordinate = ToCoordinateModel(mapInfo.WorldPosition);

                        AnchorFeature anchorPointFeature = mapInfo.Feature as AnchorFeature;
                        if (anchorPointFeature == null)
                        {
                            throw new InvalidOperationException("AnchorPointFeature is null");
                        }

                        RouteViewModel routeViewModel = (anchorPointFeature.ParentFeature as RouteFeature).ViewModel as RouteViewModel;
                        OnAddingNextWaypointToNewRoute(routeViewModel, coordinate);
                    }
                    return false;


                    //return AddVertex(mapInfo, mapControl.Map.Navigator.Viewport.ScreenToWorld(screenPosition).ToCoordinate());
                }
            case MouseState.Dragging:
                {
                    if (EditMode == WidgetEditMode.Modify)
                    {
                        var mapInfo = mapControl.GetMapInfo(screenPosition);
                        return Dragging(mapInfo?.WorldPosition?.ToPoint());
                    }

                    return false;
                }
            case MouseState.Moving:
                {
                    var mapInfo = mapControl.GetMapInfo(screenPosition);
                    HoveringVertex(mapInfo);
                }
                return false;
            case MouseState.DoubleClick:
                if (EditMode != WidgetEditMode.Modify)
                {
                    var mapInfo = mapControl.GetMapInfo(screenPosition, VertexRadius);
                    return EndEdit(mapInfo);
                }
                return false;
            default:
                throw new Exception();
        }
    }

    #endregion

    #region Edit

    #region Events
    public class InfoRequestEventArgs : EventArgs
    {
        public InfoRequestEventArgs(CoordinateModel coordinate)
        {
            Coordinate = coordinate;
        }

        public CoordinateModel Coordinate { get; }
    }

    public event EventHandler<InfoRequestEventArgs>? InfoRequest;

    protected virtual void OnInfoRequest(CoordinateModel coordinate)
    {
        if (InfoRequest != null)
        {
            InfoRequestEventArgs infoRequestEventArgs = new InfoRequestEventArgs(coordinate);
            InfoRequest(this, infoRequestEventArgs);
        }
    }


    public class AddingNewMarkerEventArgs : EventArgs
    {
        public AddingNewMarkerEventArgs(CoordinateModel coordinate)
        {
            Coordinate = coordinate;
        }

        public CoordinateModel Coordinate { get; }
    }

    public event EventHandler<AddingNewMarkerEventArgs>? AddingNewMarker;

    protected virtual void OnNewMarkerAdded(CoordinateModel coordinate)
    {
        if (AddingNewMarker != null)
        {
            AddingNewMarkerEventArgs newMarkerAddedEventArgs = new AddingNewMarkerEventArgs(coordinate);
            AddingNewMarker(this, newMarkerAddedEventArgs);
        }
    }

    public class DeleteMarkerEventArgs : EventArgs
    {
        public DeleteMarkerEventArgs(MarkerBaseViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public MarkerBaseViewModel ViewModel { get; }
    }

    public event EventHandler<DeleteMarkerEventArgs>? DeleteMarker;

    protected virtual void OnDeleteMarker(MarkerBaseViewModel viewModel)
    {
        if (DeleteMarker != null)
        {
            DeleteMarkerEventArgs newMarkerAddedEventArgs = new DeleteMarkerEventArgs(viewModel);
            DeleteMarker(this, newMarkerAddedEventArgs);
        }
    }

    public class DeleteRouteEventArgs : EventArgs
    {
        public DeleteRouteEventArgs(RouteBaseViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public RouteBaseViewModel ViewModel { get; }
    }

    public event EventHandler<DeleteRouteEventArgs>? DeleteRoute;

    protected virtual void OnDeleteRoute(RouteBaseViewModel viewModel)
    {
        if (DeleteRoute != null)
        {
            DeleteRouteEventArgs deleteRouteEventArgs = new DeleteRouteEventArgs(viewModel);
            DeleteRoute(this, deleteRouteEventArgs);
        }
    }

    public class DeleteRouteWaypointEventArgs : EventArgs
    {
        public DeleteRouteWaypointEventArgs(RouteBaseViewModel viewModel, int index)
        {
            ViewModel = viewModel;
            Index = index;
        }

        public RouteBaseViewModel ViewModel { get; }
        public int Index { get; }
    }
    public event EventHandler<DeleteRouteWaypointEventArgs>? DeleteRouteWaypoint;
    protected virtual void OnDeleteRouteWaypoint(RouteBaseViewModel viewModel, int index)
    {
        if (DeleteRouteWaypoint != null)
        {
            DeleteRouteWaypointEventArgs deleteRouteWaypointEventArgs = new DeleteRouteWaypointEventArgs(viewModel, index);
            DeleteRouteWaypoint(this, deleteRouteWaypointEventArgs);
        }
    }

    public class StartDrawingNewRouteEventArgs : EventArgs
    {
        public StartDrawingNewRouteEventArgs(CoordinateModel coordinate)
        {
            Coordinate = coordinate;
        }

        public CoordinateModel Coordinate { get; }
    }

    public event EventHandler<StartDrawingNewRouteEventArgs>? StartDrawingNewRoute;

    protected virtual void OnStartDrawingNewRoute(CoordinateModel coordinate)
    {
        if (StartDrawingNewRoute != null)
        {
            StartDrawingNewRouteEventArgs startDrawingNewRouteEventArgs = new StartDrawingNewRouteEventArgs(coordinate);
            StartDrawingNewRoute(this, startDrawingNewRouteEventArgs);
        }
    }

    public class AddingNextWaypointToNewRouteEventArgs : EventArgs
    {
        public AddingNextWaypointToNewRouteEventArgs(RouteViewModel viewModel, CoordinateModel coordinate)
        {
            Coordinate = coordinate;
            ViewModel = viewModel;
        }

        public CoordinateModel Coordinate { get; }
        public RouteViewModel ViewModel { get; }
    }

    public event EventHandler<AddingNextWaypointToNewRouteEventArgs>? AddingNextWaypointToNewRoute;

    protected virtual void OnAddingNextWaypointToNewRoute(RouteViewModel viewModel, CoordinateModel coordinate)
    {
        if (AddingNextWaypointToNewRoute != null)
        {
            AddingNextWaypointToNewRouteEventArgs drawingRouteEventArgs = new AddingNextWaypointToNewRouteEventArgs(viewModel, coordinate);
            AddingNextWaypointToNewRoute(this, drawingRouteEventArgs);
        }
    }

    public class EndDrawingNewRouteEventArgs : EventArgs
    {
        public EndDrawingNewRouteEventArgs(RouteViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public RouteViewModel ViewModel { get; }
    }

    public event EventHandler<EndDrawingNewRouteEventArgs>? EndDrawingNewRoute;

    protected virtual void OnEndDrawingNewRoute(RouteViewModel routeViewModel)
    {
        if (EndDrawingNewRoute != null)
        {
            EndDrawingNewRouteEventArgs eventArgs = new EndDrawingNewRouteEventArgs(routeViewModel);
            EndDrawingNewRoute(this, eventArgs);
        }
    }

    public class HoveringNextPointOfRouteEventArgs : EventArgs
    {
        public HoveringNextPointOfRouteEventArgs(
            RouteViewModel viewModel,
            CoordinateModel coordinate
        )
        {
            ViewModel = viewModel;
            Coordinate = coordinate;
        }
        public RouteViewModel ViewModel { get; }
        public CoordinateModel Coordinate { get; }
    }

    public event EventHandler<HoveringNextPointOfRouteEventArgs>? HoveringNextPointOfRoute;

    /// <summary>
    /// Invoked when the mouse is hovering, during the insertion of a new route.
    /// </summary>
    protected virtual void OnHoveringNextPointOfRoute(RouteViewModel viewModel, CoordinateModel coordinate)
    {
        if (HoveringNextPointOfRoute != null)
        {
            HoveringNextPointOfRouteEventArgs eventArgs = new HoveringNextPointOfRouteEventArgs(viewModel, coordinate);
            HoveringNextPointOfRoute(this, eventArgs);
        }
    }

    public class WaypointInsertedIntoRouteEventArgs : EventArgs
    {
        public WaypointInsertedIntoRouteEventArgs(RouteBaseViewModel viewModel, CoordinateModel coordinate, int index)
        {
            ViewModel = viewModel;
            Coordinate = coordinate;
            Index = index;
        }
        public RouteBaseViewModel ViewModel { get; }
        public CoordinateModel Coordinate { get; }
        public int Index { get; }
    }

    public event EventHandler<WaypointInsertedIntoRouteEventArgs>? WaypointInsertedIntoRoute;


    protected virtual void OnWaypointInsertedIntoRoute(RouteBaseViewModel routeViewModel, CoordinateModel coordinate, int index)
    {
        if (WaypointInsertedIntoRoute != null)
        {
            WaypointInsertedIntoRouteEventArgs waypointInsertedIntoRouteEventArgs = new WaypointInsertedIntoRouteEventArgs(routeViewModel, coordinate, index);
            WaypointInsertedIntoRoute(this, waypointInsertedIntoRouteEventArgs);
        }
    }

    #endregion
    [Obsolete]
    public class MyAddInfo
    {
        public MyAddInfo(
            RouteFeature? editingFeature,
            CoordinateModel[]? coordinates,
            int? editingIndex
        )
        {
            EditingIndex = editingIndex;
            Coordinates = coordinates;
            EditingFeature = editingFeature;
        }

        public CoordinateModel[]? Coordinates { get; private set; }
        public int? EditingIndex { get; private set; }
        public RouteFeature? EditingFeature { get; private set; }

        public void Update(
            RouteFeature? editingFeature,
            CoordinateModel[]? coordinates,
            int? editingIndex
        )
        {
            EditingIndex = editingIndex;
            Coordinates = coordinates;
            EditingFeature = editingFeature;
        }
    }

    [Obsolete]
    private readonly DragInfo _dragInfo = new();
    [Obsolete]
    private readonly MyAddInfo _myAddInfo = new(null, null, null);

    public int VertexRadius { get; set; } = 12;
    private bool EndEdit(MapInfo mapInfo)
    {
        if (_myAddInfo.EditingIndex is null) return false;
        if (_myAddInfo.Coordinates is null) return false;

        if (EditMode == WidgetEditMode.DrawingRoute)
        {
            _myAddInfo.Update(null, null, null);
            EditMode = WidgetEditMode.AddRoute;

            AnchorFeature anchorPointFeature = mapInfo.Feature as AnchorFeature;
            if (anchorPointFeature == null)
            {
                throw new InvalidOperationException("Anchor is null");
            }
            RouteViewModel viewModel = (anchorPointFeature.ParentFeature as RouteFeature).ViewModel as RouteViewModel;

            OnEndDrawingNewRoute(viewModel);
        }
        return false;
    }

    //private static CoordinateModel ToGeoCoordinate(MPoint worldPoint)
    //{
    //    var lonLat = SphericalMercator.ToLonLat(worldPoint);
    //    CoordinateModel coordinate = new CoordinateModel(lonLat.Y, lonLat.X);
    //    return coordinate;
    //}
    private static CoordinateModel ToCoordinateModel(MPoint worldPoint)
    {
        var lonLat = SphericalMercator.ToLonLat(worldPoint);
        CoordinateModel coordinate = new CoordinateModel()
        {
            Latitude = lonLat.Y,
            Longitude = lonLat.X
        };
        return coordinate;
    }

    private static MPoint FromGeoCoordinate(CoordinateModel coordinate)
    {
        var worldPoint = SphericalMercator.FromLonLat(coordinate.Longitude, coordinate.Latitude);
        return worldPoint.ToMPoint();
    }

    public void HoveringVertex(MapInfo? mapInfo)
    {
        if (mapInfo != null)
        {
            if (_myAddInfo.EditingIndex != null)
            {
                if (_myAddInfo.Coordinates == null)
                {
                    throw new InvalidOperationException($"Inconsistency detected: property {nameof(_myAddInfo.EditingIndex)} is not null, but property {nameof(_myAddInfo.Coordinates)} is null");
                }

                CoordinateModel coordinate = ToCoordinateModel(mapInfo.WorldPosition);

                _myAddInfo.Coordinates[_myAddInfo.EditingIndex.Value] = coordinate;
                if (EditMode == WidgetEditMode.DrawingRoute)
                {
                    RouteViewModel viewModel = _myAddInfo.EditingFeature.ViewModel as RouteViewModel;

                    OnHoveringNextPointOfRoute(viewModel, coordinate);
                }
            }
        }
    }


    // TODO: manage IEnumerable<MarkerBaseViewModel>
    public void AddMarkerToUi(MarkerBaseViewModel markerViewModelBase)
    {
        CoordinateModel coordinate = markerViewModelBase.Coordinate.ToCoordinateModel();
        var worldPosition = FromGeoCoordinate(coordinate);

        var geometry = worldPosition.ToPoint();
        var newFeature = new MarkerFeature(
            markerViewModelBase,
            geometry
            );

        Layer?.Add(newFeature);
    }

    // TODO: manage IEnumerable<RouteViewModelBase>
    public void AddRouteToUi(RouteBaseViewModel routeViewModelBase)
    {
        var newFeature = new RouteFeature(routeViewModelBase);

        var coordinates = routeViewModelBase
            .Coordinates
            .Select(c => c.ToCoordinateModel())
            .ToArray();
        _myAddInfo.Update(newFeature, coordinates, 1);

        Layer?.Add(newFeature);
    }

    public void RemoveRouteFromUi(IEnumerable<RouteBaseViewModel> itemsToRemove)
    {
        var itemsToRemoveHashSet = itemsToRemove.ToHashSet();
        var layer = Layer;
        if (layer != null)
        {
            var featureToRemove = layer
                .GetFeatures()
                .OfType<RouteFeature>()
                .SingleOrDefault(f => itemsToRemoveHashSet.Contains(f.ViewModel));

            if (featureToRemove != null)
            {
                if (layer.TryRemove(featureToRemove))
                {
                    featureToRemove.DetachEvents();
                }
                else
                {
                    throw new InvalidOperationException("Unable to remove feature.");
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to retrieve feature to remove.");
            }
        }
        else
        {
            throw new InvalidOperationException("Unable to retrieve layer.");
        }
    }

    public void ResetRoutesOnUi()
    {
        var featuresToRemove = Layer?
            .GetFeatures()
            .OfType<RouteFeature>()
            .ToList();
        if (featuresToRemove != null)
        {
            foreach (var featureToRemove in featuresToRemove)
            {
                Layer?.TryRemove(featureToRemove);
            }
        }
    }

    public void RemoveMarkerFromUi(IEnumerable<MarkerBaseViewModel> itemsToRemove)
    {
        var layer = Layer;
        if (layer != null)
        {
            var itemsToRemoveHashSet = itemsToRemove.ToHashSet();
            var featureToRemove = layer
                .GetFeatures()
                .OfType<MarkerFeature>()
                .SingleOrDefault(f => itemsToRemoveHashSet.Contains(f.ViewModel));

            if (featureToRemove != null)
            {
                if (layer.TryRemove(featureToRemove))
                {
                    featureToRemove.DetachEvents();
                }
                else
                {
                    throw new InvalidOperationException("Unable to remove feature.");
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to retrieve feature to remove.");
            }
        }
        else
        {
            throw new InvalidOperationException("Unable to retrieve layer.");
        }
    }

    public void ResetMarkersOnUi()
    {
        var featuresToRemove = Layer?
            .GetFeatures()
            .OfType<MarkerFeature>()
            .ToList();
        if (featuresToRemove != null)
        {
            foreach (var featureToRemove in featuresToRemove)
            {
                Layer?.TryRemove(featureToRemove);
            }
        }
    }

    private void InsertWaypointIntoRoute(RouteFeature geometryFeature, Coordinate worldPosition, int index)
    {
        var coordinateList = geometryFeature.Geometry.Coordinates.ToList();
        var viewModel = geometryFeature.ViewModel;
        if (viewModel == null)
        {
            throw new InvalidOperationException("ViewModel is unexpectedly null.");
        }
        coordinateList.Insert(index, worldPosition);
        var editingIndex = index;
        var coordinates = coordinateList
            .Select(c => ToCoordinateModel(c.ToMPoint()))
            .ToArray();
        _myAddInfo.Update(geometryFeature, coordinates, editingIndex);

        CoordinateModel coordinate = ToCoordinateModel(worldPosition.ToMPoint());

        OnWaypointInsertedIntoRoute(viewModel, coordinate, index);
    }

    //private bool AddVertex(MapInfo mapInfo, Coordinate worldPosition)
    //{
    //    if (EditMode == WidgetEditMode.AddMarker)
    //    {
    //        CoordinateModel coordinate = ToCoordinateModel(worldPosition.ToMPoint());
    //        OnNewMarkerAdded(coordinate);
    //    }
    //    else if (EditMode == WidgetEditMode.AddRoute)
    //    {
    //        CoordinateModel coordinate = ToCoordinateModel(worldPosition.ToMPoint());

    //        OnStartDrawingNewRoute(coordinate);

    //        EditMode = WidgetEditMode.DrawingRoute;
    //    }
    //    else if (EditMode == WidgetEditMode.DrawingRoute)
    //    {
    //        if (_myAddInfo.EditingIndex == null) return false;
    //        if (_myAddInfo.Coordinates == null) return false;

    //        CoordinateModel coordinate = ToCoordinateModel(worldPosition.ToMPoint());

    //        AnchorFeature anchorPointFeature = mapInfo.Feature as AnchorFeature;
    //        if (anchorPointFeature == null)
    //        {
    //            throw new InvalidOperationException("AnchorPointFeature is null");
    //        }

    //        RouteViewModel routeViewModel = (anchorPointFeature.ParentFeature as RouteFeature).ViewModel as RouteViewModel;
    //        OnAddingNextWaypointToNewRoute(routeViewModel, coordinate);
    //    }
    //    return false;
    //}

    private static Coordinate? FindVertexTouched(MapInfo mapInfo, IEnumerable<Coordinate> vertices, double screenDistance)
    {
        if (mapInfo.WorldPosition == null)
            return null;

        return vertices.OrderBy(v => v.Distance(mapInfo.WorldPosition.ToCoordinate()))
            .FirstOrDefault(v => v.Distance(mapInfo.WorldPosition.ToCoordinate()) < mapInfo.Resolution * screenDistance);
    }

    private bool StartDragging(MapInfo mapInfo, double screenDistance)
    {
        if (mapInfo.Feature != null)
        {
            if (mapInfo.Feature is GeometryFeature geometryFeature)
            {
                var vertexTouched = FindVertexTouched(mapInfo, geometryFeature.Geometry?.MainCoordinates() ?? new List<Coordinate>(), screenDistance);
                _dragInfo.Feature = geometryFeature;
                _dragInfo.Vertex = vertexTouched;
                if (mapInfo.WorldPosition != null)
                {
                    if (_dragInfo.Vertex != null)
                    {
                        _dragInfo.DraggingFeature = false;
                        _dragInfo.StartOffsetToVertex = mapInfo.WorldPosition - _dragInfo.Vertex.ToMPoint();

                        CoordinateModel mouseCoordinate = ToCoordinateModel(mapInfo.WorldPosition);

                        if (geometryFeature is AnchorFeature anchorFeature)
                        {
                            RouteViewModel editingRouteViewModel = (anchorFeature.ParentFeature as RouteFeature).ViewModel as RouteViewModel;
                            OnStartModifyingRoute(editingRouteViewModel, anchorFeature.Index, mouseCoordinate);
                            return true; // to indicate start of drag
                        }
                    }
                    if (geometryFeature is MarkerFeature markerFeature)
                    {
                        _dragInfo.StartOffsetToVertex = mapInfo.WorldPosition - mapInfo.Feature.Extent.Centroid;
                        _dragInfo.Vertex = mapInfo.Feature.Extent.Centroid.ToCoordinate();
                        _dragInfo.DraggingFeature = true;

                        CoordinateModel mouseCoordinate = ToCoordinateModel(mapInfo.WorldPosition);

                        OnStartMovingMarker(markerFeature.ViewModel as MarkerViewModel, mouseCoordinate);
                        return true; // to indicate start of drag
                    }
                }
            }
        }

        return false;
    }

    public class StartMovingMarkerEventArgs : EventArgs
    {
        public StartMovingMarkerEventArgs(MarkerViewModel viewModel, CoordinateModel mouseCoordinate)
        {
            MouseCoordinate = mouseCoordinate;
            ViewModel = viewModel;
        }

        public CoordinateModel MouseCoordinate { get; }
        public MarkerViewModel ViewModel { get; }
    }

    public event EventHandler<StartMovingMarkerEventArgs>? StartMovingMarker;

    protected virtual void OnStartMovingMarker(MarkerViewModel viewModel, CoordinateModel mouseCoordinate)
    {
        if (StartMovingMarker != null)
        {
            StartMovingMarkerEventArgs startMovingMarkerEventArgs = new StartMovingMarkerEventArgs(viewModel, mouseCoordinate);
            StartMovingMarker(this, startMovingMarkerEventArgs);
        }
    }

    public class StartModifyingRouteEventArgs : EventArgs
    {
        public StartModifyingRouteEventArgs(RouteViewModel viewModel, int index, CoordinateModel mouseCoordinate)
        {
            Index = index;
            MouseCoordinate = mouseCoordinate;
            ViewModel = viewModel;
        }

        public RouteViewModel ViewModel { get; }
        public int Index { get; }
        public CoordinateModel MouseCoordinate { get; }
    }

    public event EventHandler<StartModifyingRouteEventArgs>? StartModifyingRoute;

    protected virtual void OnStartModifyingRoute(RouteViewModel viewModel, int index, CoordinateModel mouseCoordinate)
    {
        if (StartModifyingRoute != null)
        {
            StartModifyingRouteEventArgs startModifyingRouteEventArgs = new StartModifyingRouteEventArgs(viewModel, index, mouseCoordinate);
            StartModifyingRoute(this, startModifyingRouteEventArgs);
        }
    }

    public class ModifyingRouteEventArgs : EventArgs
    {
        public ModifyingRouteEventArgs(RouteBaseViewModel routeViewModel, int index, CoordinateModel coordinate)
        {
            ViewModel = routeViewModel;
            Index = index;
            Coordinate = coordinate;
        }

        public RouteBaseViewModel ViewModel { get; }
        public int Index { get; }
        public CoordinateModel Coordinate { get; }
    }

    public event EventHandler<ModifyingRouteEventArgs>? ModifyingRoute;

    protected virtual void OnModifyingRoute(RouteBaseViewModel viewModel, int index, CoordinateModel coordinate)
    {
        if (ModifyingRoute != null)
        {
            ModifyingRouteEventArgs modifyingRouteEventArgs = new ModifyingRouteEventArgs(viewModel, index, coordinate);
            ModifyingRoute(this, modifyingRouteEventArgs);
        }
    }

    public class MovingMarkerEventArgs : EventArgs
    {
        public MovingMarkerEventArgs(MarkerBaseViewModel viewModel, CoordinateModel newCoordinate)
        {
            NewCoordinate = newCoordinate;
            ViewModel = viewModel;
        }

        public CoordinateModel NewCoordinate { get; }
        public MarkerBaseViewModel ViewModel { get; }
    }

    public event EventHandler<MovingMarkerEventArgs>? MovingMarker;

    protected virtual void OnMovingMarker(MarkerBaseViewModel viewModel, CoordinateModel coordinate)
    {
        if (MovingMarker != null)
        {
            MovingMarkerEventArgs movingMarkerEventArgs = new MovingMarkerEventArgs(viewModel, coordinate);
            MovingMarker(this, movingMarkerEventArgs);
        }
    }
    public class EndMovingMarkerEventArgs : EventArgs
    {
        public EndMovingMarkerEventArgs(MarkerViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public MarkerViewModel ViewModel { get; }
    }

    public event EventHandler<EndMovingMarkerEventArgs>? EndMovingMarker;

    protected virtual void OnEndMovingMarker(MarkerViewModel viewModel)
    {
        if (EndMovingMarker != null)
        {
            EndMovingMarkerEventArgs endMovingMarkerEventArgs = new EndMovingMarkerEventArgs(viewModel);
            EndMovingMarker(this, endMovingMarkerEventArgs);
        }
    }

    public class EndModifyingRouteEventArgs : EventArgs
    {
        public EndModifyingRouteEventArgs(RouteViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        public RouteViewModel ViewModel { get; }
    }

    public event EventHandler<EndModifyingRouteEventArgs>? EndModifyingRoute;

    protected virtual void OnEndModifyingRoute(RouteViewModel viewModel)
    {
        if (EndModifyingRoute != null)
        {
            EndModifyingRouteEventArgs endModifyingRouteEventArgs = new EndModifyingRouteEventArgs(viewModel);
            EndModifyingRoute(this, endModifyingRouteEventArgs);
        }
    }

    private bool Dragging(Point? worldPosition)
    {
        if (EditMode != WidgetEditMode.Modify || _dragInfo.Feature == null || worldPosition == null || (_dragInfo.StartOffsetToVertex == null)) return false;

        if (_dragInfo.Vertex != null)
        {
            // only modify the vertex if it is not moving a feature
            if (!_dragInfo.DraggingFeature)
            {
                if (_dragInfo.Feature is MarkerFeature markerFeature)
                {
                    return true;
                }
                AnchorFeature anchorPointFeature = _dragInfo.Feature as AnchorFeature;
                if (anchorPointFeature == null)
                {
                    throw new InvalidOperationException("unable to perform drag operation");
                }
                var parentFeature = anchorPointFeature.ParentFeature;
                var viewModel = (parentFeature as RouteFeature).ViewModel;
                if (viewModel == null)
                {
                    throw new InvalidOperationException("ViewModel is unexpectedly null");
                }
                var newCoordinate = ToCoordinateModel(worldPosition.ToMPoint() - _dragInfo.StartOffsetToVertex);
                OnModifyingRoute(viewModel, anchorPointFeature.Index, newCoordinate);
            }
            else
            {
                if (_dragInfo.Feature is MarkerFeature markerFeature)
                {
                    var viewModel = markerFeature.ViewModel as MarkerViewModel;
                    if (viewModel == null)
                    {
                        throw new InvalidOperationException("ViewModel is unexpectedly null");
                    }

                    var startingPoint = viewModel.MouseStartDraggingCoordinate;
                    var previousVertex1 = FromGeoCoordinate(startingPoint);

                    var delta = worldPosition.ToMPoint() - previousVertex1;
                    var worldPosition2 = FromGeoCoordinate(viewModel.CoordinateAtDraggingStart);
                    var newWorldPoint = worldPosition2 + delta;
                    var newCoordinate = ToCoordinateModel(newWorldPoint);
                    OnMovingMarker(viewModel, newCoordinate);
                }
                else
                {
                    throw new InvalidOperationException("Don't know what to move");
                }
            }
        }

        Layer?.DataHasChanged();
        return true;
    }

    private void StopDragging()
    {
        if (EditMode == WidgetEditMode.Modify && _dragInfo.Feature != null)
        {
            if (_dragInfo.Feature is MarkerFeature markerFeature)
            {
                OnEndMovingMarker(markerFeature.ViewModel as MarkerViewModel);
            }
            else if (_dragInfo.Feature is AnchorFeature anchorFeature)
            {
                RouteViewModel editingRouteViewModel = (anchorFeature.ParentFeature as RouteFeature).ViewModel as RouteViewModel;
                OnEndModifyingRoute(editingRouteViewModel);
            }

            _dragInfo.Feature.Geometry?.GeometryChanged();
            _dragInfo.Feature = null;
        }
    }

    #endregion
}
