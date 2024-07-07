using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Fly.Features;
using Fly.ViewModels;
using Fly.Widgets;
using Brush = Mapsui.Styles.Brush;
using Color = Mapsui.Styles.Color;
using Pen = Mapsui.Styles.Pen;
using Point = NetTopologySuite.Geometries.Point;
using Map = Mapsui.Map;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System;
using Avalonia;
using Mapsui.UI.Avalonia;
using Avalonia.Interactivity;
using Fly.StyleRenderers;
using Fly.Tiling;

namespace Fly.AttachedProperties;


public class MapControlAttachedProperties : AvaloniaObject
{

    #region Layers-related
    private static readonly Color _editModeColor = new(10, 106, 167, 150);
    private static readonly Color _selectedPinColor = new(255, 125, 57);
    private static readonly Color _normalPinColor = new Color(21, 149, 223);

    private static readonly Dictionary<MapLayerViewModelKey, LayerInfosBag> _layerInfosBags;
    #endregion

    static MapControlAttachedProperties()
    {
        _layerInfosBags = new Dictionary<MapLayerViewModelKey, LayerInfosBag>();

        MapRotationProperty.Changed.AddClassHandler<MapControl>(MapRotationCallback);
        LayersProperty.Changed.AddClassHandler<MapControl>(LayersCallback);
        MapEditToolProperty.Changed.AddClassHandler<MapControl>(MapEditToolCallback);
        MarkersProperty.Changed.AddClassHandler<MapControl>(MarkersCallback);
        RoutesProperty.Changed.AddClassHandler<MapControl>(RoutesCallback);        
    }

    #region Rotation
    public static double GetMapRotation(MapControl d)
    {
        return (double)d.GetValue(MapRotationProperty);
    }
    public static void SetMapRotation(MapControl d, double value)
    {
        d.SetValue(MapRotationProperty, value);
    }

    public static readonly AttachedProperty<double> MapRotationProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, double>(
        "MapRotation", 0.0);

    private static void MapRotationCallback(MapControl mapControl, AvaloniaPropertyChangedEventArgs e)
    {
        if (mapControl != null)
        {
            mapControl.Map.Navigator.RotateTo(e.NewValue != null ? (double)e.NewValue : 0.0);
            mapControl.InvalidateVisual();
        }
    }
    #endregion

    #region EditMode
    public static MapEditTools GetMapEditTool(MapControl d)
    {
        return d.GetValue(MapEditToolProperty);
    }
    public static void SetMapEditTool(MapControl d, MapEditTools value)
    {
        d.SetValue(MapEditToolProperty, value);
    }

    public static readonly AttachedProperty<MapEditTools> MapEditToolProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, MapEditTools>(
        "MapEditTool", MapEditTools.None);

    private static void MapEditToolCallback(MapControl mapControl, AvaloniaPropertyChangedEventArgs e)
    {
        if (mapControl != null)
        {
            var map = mapControl.Map;
            if (map != null)
            {
                var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
                if (flyEditingWidget != null)
                {
                    var newEditModes = ToEditMode((MapEditTools)e.NewValue);
                    flyEditingWidget.EditMode = newEditModes;
                }
            }
        }
    }

    private static WidgetEditMode ToEditMode(MapEditTools e)
    {
        switch (e)
        {
            case MapEditTools.Modify:
                return WidgetEditMode.Modify;
            case MapEditTools.AddRoute:
                return WidgetEditMode.AddRoute;
            case MapEditTools.AddMarker:
                return WidgetEditMode.AddMarker;
            case MapEditTools.Delete:
                return WidgetEditMode.Delete;
            case MapEditTools.None:
                return WidgetEditMode.None;
            default:
                throw new NotSupportedException($"Unable to convert {e} to type {typeof(WidgetEditMode)}.");
        }
    }
    #endregion   

    #region Layers

    private class MapLayerViewModelKey : IEquatable<MapLayerViewModelKey>
    {
        public MapLayerViewModelKey(
            Map map,
            LayerBaseViewModel viewModel
        )
        {
            Map = map;
            ViewModel = viewModel;
        }
        Map Map { get; }
        LayerBaseViewModel ViewModel { get; }

        public bool Equals(MapLayerViewModelKey? other)
        {
            if (!Equals(null, other))
            {
                if (ReferenceEquals(Map, other.Map))
                {
                    if (ReferenceEquals(ViewModel, other.ViewModel))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return (obj is MapLayerViewModelKey mapLayerViewModelKey) && Equals(mapLayerViewModelKey);
        }

        public override int GetHashCode()
        {
            return Map.GetHashCode() ^ ViewModel.GetHashCode();
        }

        public static bool operator ==(MapLayerViewModelKey obj1, MapLayerViewModelKey obj2)
        {
            return obj1.Equals((object)obj2);
        }
        public static bool operator !=(MapLayerViewModelKey obj1, MapLayerViewModelKey obj2)
        {
            return !obj1.Equals((object)obj2);
        }
    }

    public static void SetLayers(MapControl d, INotifyCollectionChanged value)
    {
        d.SetValue(LayersProperty, value);
    }

    public static ObservableCollection<LayerBaseViewModel> GetLayers(MapControl d)
    {
        return d.GetValue(LayersProperty);
    }

    public static readonly AttachedProperty<ObservableCollection<LayerBaseViewModel>> LayersProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ObservableCollection<LayerBaseViewModel>>(
            "Layers", new ObservableCollection<LayerBaseViewModel>());

    private static void LayersCallback(MapControl mapControl, AvaloniaPropertyChangedEventArgs e)
    {
        var map = mapControl.Map;
        if (map != null)
        {
            if (e.OldValue is ObservableCollection<LayerBaseViewModel> oldLayers)
            {
                oldLayers.CollectionChanged -= (s, ea) => NewLayers_CollectionChanged(mapControl, s, ea);
                RemoveExistingLayersFromControl(map, oldLayers);
            }

            if (e.NewValue is ObservableCollection<LayerBaseViewModel> newLayers)
            {
                AddNewLayersToControl(mapControl, newLayers, 0);
                newLayers.CollectionChanged += (s, ea) => NewLayers_CollectionChanged(mapControl, s, ea);
            }
        }
    }

    private static void NewLayers_CollectionChanged(MapControl mapControl, object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                var newItems = e.NewItems.OfType<LayerBaseViewModel>();
                AddNewLayersToControl(mapControl, newItems, e.NewStartingIndex);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                var oldItems = e.OldItems.OfType<LayerBaseViewModel>();
                RemoveExistingLayersFromControl(mapControl.Map, oldItems);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Move)
        {
            if (e.NewItems != null)
            {
                MoveLayersInControl(mapControl.Map, e.NewItems.OfType<LayerBaseViewModel>(), e.OldStartingIndex, e.NewStartingIndex);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            throw new NotImplementedException();
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private static LayerInfosBag GetLayerInfo(Map map, LayerBaseViewModel layerViewModel)
    {
        MapLayerViewModelKey key = new MapLayerViewModelKey(map, layerViewModel);

        LayerInfosBag layerInfosBag;
        lock (_layerInfosBags)
        {
            LayerInfosBag? val;
            if (!_layerInfosBags.TryGetValue(key, out val))
            {
                throw new InvalidOperationException("Key is unexpectedly missing.");
            }
            if (val == null)
            {
                throw new InvalidOperationException("Value is null.");
            }
            layerInfosBag = val;
        }
        return layerInfosBag;
    }

    private static void RemoveExistingLayersFromControl(Map map, IEnumerable<LayerBaseViewModel> oldLayerViewModels)
    {
        List<ILayer> oldLayers = new List<ILayer>();
        foreach (LayerBaseViewModel oldLayerViewModel in oldLayerViewModels)
        {
            LayerInfosBag layerInfosBag = GetLayerInfo(map, oldLayerViewModel);
            layerInfosBag.Unassociate(map);
            oldLayers.Add(layerInfosBag.Layer);

            MapLayerViewModelKey key = new MapLayerViewModelKey(map, oldLayerViewModel);
            lock (_layerInfosBags)
            {
                _layerInfosBags.Remove(key);
            }
        }
        map.Layers.Remove(oldLayers.ToArray());
    }

    /// <summary>
    /// Adds the new layers to control.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="newLayerViewModels">The new layer view models.</param>
    /// <param name="newStartingIndex">If is not -1, then it contains the index where the new items were added.</param>
    private static void AddNewLayersToControl(
        MapControl mapControl,
        IEnumerable<LayerBaseViewModel> newLayerViewModels,
        int newStartingIndex
        )
    {
        List<ILayer> newLayers = new List<ILayer>();
        foreach (var newLayerViewModel in newLayerViewModels)
        {
            ILayer layer = ToLayer(newLayerViewModel);
            if (layer != null)
            {
                LayerInfosBag layerInfosBag = newLayerViewModel.CreateLayerInfosBag(layer, mapControl);
                layerInfosBag.Associate(mapControl.Map);

                MapLayerViewModelKey key = new MapLayerViewModelKey(mapControl.Map, newLayerViewModel);
                lock (_layerInfosBags)
                {
                    _layerInfosBags.Add(key, layerInfosBag);
                }
                newLayers.Add(layer);
            }
        }
        if (newLayers.Any())
        {
            mapControl.Map.Layers.Insert(newStartingIndex, newLayers.ToArray());
        }
    }

    private static void MoveLayersInControl(
        Map map,
        IEnumerable<LayerBaseViewModel> layerViewModelsToMove,
        int oldStartingIndex,
        int newStartingIndex
        )
    {
        int newIndex = newStartingIndex;
        foreach (var newLayerViewModel in layerViewModelsToMove)
        {
            LayerInfosBag layerInfosBag = GetLayerInfo(map, newLayerViewModel);
            ILayer layer = layerInfosBag.Layer;
            map.Layers.Move(newIndex, layer);
            newIndex++;
        }
    }

    private class MyWritableLayer : WritableLayer
    {
        public override IEnumerable<IFeature> GetFeatures(MRect? box, double resolution)
        {
            List<GeometryFeature> list = base
                .GetFeatures(box, resolution)
                .Cast<GeometryFeature>()
                .ToList();

            foreach (GeometryFeature item in list)
            {
                yield return item;


                #region also return the set of Vertices for routes
                if (item is RouteFeature routeFeature)
                {
                    for (int i = 0; i < routeFeature.ViewModel.Coordinates.Count; i++)
                    {
                        var coordinateViewModel = routeFeature.ViewModel.Coordinates[i];
                        var v = Mapsui.Projections.SphericalMercator.FromLonLat(coordinateViewModel.Longitude, coordinateViewModel.Latitude);
                        AnchorFeature geometryFeature = new AnchorFeature(item, item.Geometry, i)
                        {
                            Geometry = new Point(v.x, v.y)
                        };
                        yield return geometryFeature;
                    }
                }
                #endregion
            }
        }
    }

    private static ILayer ToLayer(LayerBaseViewModel layerViewModel)
    {
        ILayer result;
        if (layerViewModel is OpenStreetMapLayerViewModel openStreetMapLayerViewModel)
        {
            result = OpenStreetMap.CreateTileLayer(
                openStreetMapLayerViewModel.UrlFormatter,
                openStreetMapLayerViewModel.UserAgent
                );
            result.Name = layerViewModel.DisplayName;
        }
        else if (layerViewModel is MapTilerSatelliteLayerViewModel mapTilerSatelliteLayerViewModel)
        {
            if(string.IsNullOrWhiteSpace(mapTilerSatelliteLayerViewModel.ApiKey))
            {
                return null;
            }
            result = MapTilerSatellite.CreateTileLayer(
                mapTilerSatelliteLayerViewModel.ApiKey,
                mapTilerSatelliteLayerViewModel.UrlFormatter,
                mapTilerSatelliteLayerViewModel.UserAgent
                );
            result.Name = layerViewModel.DisplayName;
        }
        else if (layerViewModel is OpenAIPLayerViewModel openAIPLayerViewModel)
        {
            result = OpenAip.CreateTileLayer(
                openAIPLayerViewModel.ApiKey,
                openAIPLayerViewModel.UrlFormatter,
                openAIPLayerViewModel.ServersList,
                openAIPLayerViewModel.UserAgent
                );
            result.Name = layerViewModel.DisplayName;
        }
        else if (layerViewModel is FeaturesLayerViewModel featuresLayerViewModel)
        {
            result = new MyWritableLayer()
            {
                Name = "Features",
                Style = new ThemeStyle(feature =>
                {
                    if (feature is MarkerFeature markerFeature)
                    {
                        bool isVisible = markerFeature.ViewModel.IsVisible;
                        IStyle result = new StyleCollection()
                        {
                            Styles =
                                {
                                     new LabelStyle
                                     {
                                         Offset= new Offset(0, -40),
                                         ForeColor = Color.Black,
                                         BackColor = new Brush(Color.White),
                                         LabelMethod = ff => {
                                             return markerFeature.ViewModel.Coordinate.DisplayName;
                                         },
                                         Enabled = isVisible
                                     },

                                     // TODO: use viewmodel.IsSelected, instead
                                     ((bool?)feature["Selected"] == true) ?
                                         FlyMarkerStyles.CreatePinStyle(isVisible, _selectedPinColor, .5):
                                         FlyMarkerStyles.CreatePinStyle(isVisible, _normalPinColor, .5)
                                }
                        };
                        return result;
                    }
                    else if (feature is RouteFeature routeFeature)
                    {
                        bool isVisible = routeFeature.ViewModel.IsVisible;
                        IStyle result = new StyleCollection()
                        {
                            Styles =
                                    {
                                         new VectorStyle
                                         {
                                             Fill = new Brush(_editModeColor),
                                             Line = new Pen(_editModeColor, 5),
                                             Outline = new Pen(_editModeColor, 5),
                                             Enabled = isVisible
                                         },
                                         new FlyStyle()
                                         {
                                         }
                                    }
                        };
                        return result;
                    }
                    else if (feature is AnchorFeature anchorFeature)
                    {
                        bool isVisible;
                        var parent = anchorFeature.ParentFeature;
                        if (parent is RouteFeature parentRouteFeature)
                        {
                            isVisible = parentRouteFeature.ViewModel.IsVisible;
                        }
                        else
                        {
                            isVisible = true;
                        }
                        IStyle result = new SymbolStyle
                        {
                            SymbolScale = 0.5,
                            Enabled = isVisible
                        };
                        return result;
                    }
                    throw new NotImplementedException();
                }),
                IsMapInfoLayer = true
            };

            result.Name = layerViewModel.DisplayName;
        }
        else
        {
            throw new NotSupportedException($"Layers of type '{layerViewModel.GetType()}' are not supported.");
        }       

        return result;
    }

    #endregion

    #region Markers
    public static void SetMarkers(MapControl d, INotifyCollectionChanged value)
    {
        d.SetValue(MarkersProperty, value);
    }

    public static ObservableCollection<MarkerBaseViewModel> GetMarkers(MapControl d)
    {
        return d.GetValue(MarkersProperty);
    }

    public static readonly AttachedProperty<ObservableCollection<MarkerBaseViewModel>> MarkersProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ObservableCollection<MarkerBaseViewModel>>(
            "Markers", new ObservableCollection<MarkerBaseViewModel>());

    private static void MarkersCallback(MapControl mapControl, AvaloniaPropertyChangedEventArgs e)
    {
        var map = mapControl.Map;
        if (map != null)
        {
            //if (e.OldValue is ObservableCollection<MarkerViewModelBase> oldMarkers)
            //{
            //    oldMarkers.CollectionChanged -= (s, e) => NewMarkers_CollectionChanged(map, s, e);
            //    RemoveExistingMarkersFromControl(map, oldMarkers);
            //}

            if (e.NewValue is ObservableCollection<MarkerBaseViewModel> newMarkers)
            {
                AddNewMarkersToControl(map, newMarkers);
                newMarkers.CollectionChanged += (s, e) => NewMarkers_CollectionChanged(map, s, e);
            }
        }
    }

    private static void NewMarkers_CollectionChanged(Map map, object? _, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                var newItems = e.NewItems.OfType<MarkerBaseViewModel>();
                AddNewMarkersToControl(map, newItems);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                var oldItems = e.OldItems.OfType<MarkerBaseViewModel>();
                RemoveExistingMarkersFromControl(map, oldItems);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            ResetMarkersOnControl(map);
        }
        //else if (e.Action == NotifyCollectionChangedAction.Move)
        //{
        //    if (e.NewItems != null)
        //    {
        //        MoveMarkersInControl(map, e.NewItems.OfType<LayerViewModelBase>(), e.OldStartingIndex, e.NewStartingIndex);
        //    }
        //}
        else
        {
            throw new NotImplementedException();
        }
    }

    private static void AddNewMarkersToControl(
        Map map,
        IEnumerable<MarkerBaseViewModel> newViewModels
    )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            foreach (var newViewModel in newViewModels)
            {
                flyEditingWidget.AddMarkerToUi(newViewModel);
                newViewModel.PropertyChanged += (s, e) => { flyEditingWidget.Invalidate(); };
            }
            flyEditingWidget.Invalidate();
        }
    }

    private static void RemoveExistingMarkersFromControl(
       Map map,
       IEnumerable<MarkerBaseViewModel> oldViewModels
   )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            flyEditingWidget.RemoveMarkerFromUi(oldViewModels);
            foreach (var oldViewModel in oldViewModels)
            {
                oldViewModel.PropertyChanged -= (s, e) => { flyEditingWidget.Invalidate(); };
            }
            flyEditingWidget.Invalidate();
        }
    }

    private static void ResetMarkersOnControl(
        Map map
    )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            flyEditingWidget.ResetMarkersOnUi();
            flyEditingWidget.Invalidate();
        }
    }

    #endregion

    #region Routes
    public static void SetRoutes(MapControl d, INotifyCollectionChanged value)
    {
        d.SetValue(RoutesProperty, value);
    }

    public static ObservableCollection<RouteBaseViewModel> GetRoutes(MapControl d)
    {
        return d.GetValue(RoutesProperty);
    }

    public static readonly AttachedProperty<ObservableCollection<RouteBaseViewModel>> RoutesProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ObservableCollection<RouteBaseViewModel>>(
            "Routes", new ObservableCollection<RouteBaseViewModel>());

    private static void RoutesCallback(MapControl mapControl, AvaloniaPropertyChangedEventArgs e)
    {
        var map = mapControl.Map;
        if (map != null)
        {

            //TODO: manage old routes

            //if (e.OldValue is ObservableCollection<RouteBaseViewModel> oldRoutes)
            //{
            //    oldRoutes.CollectionChanged -= (s, e) => NewRoutes_CollectionChanged(map, s, e);
            //    RemoveExistingRoutesFromControl(map, oldRoutes);
            //}

            if (e.NewValue is ObservableCollection<RouteBaseViewModel> newRoutes)
            {
                AddNewRoutesToControl(map, newRoutes);
                newRoutes.CollectionChanged += (s, e) => NewRoutes_CollectionChanged(map, s, e);
            }
        }
    }

    private static void NewRoutes_CollectionChanged(Map map, object? _, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            if (e.NewItems != null)
            {
                var newItems = e.NewItems.OfType<RouteBaseViewModel>();
                AddNewRoutesToControl(map, newItems);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                var oldItems = e.OldItems.OfType<RouteBaseViewModel>();
                RemoveExistingRoutesFromControl(map, oldItems);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            ResetRoutesOnControl(map);
        }
        //else if (e.Action == NotifyCollectionChangedAction.Move)
        //{
        //    if (e.NewItems != null)
        //    {
        //        MoveRoutesInControl(map, e.NewItems.OfType<LayerViewModelBase>(), e.OldStartingIndex, e.NewStartingIndex);
        //    }
        //}
        else
        {
            throw new NotImplementedException();
        }
    }

    private static void AddNewRoutesToControl(
        Map map,
        IEnumerable<RouteBaseViewModel> newViewModels
    )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            foreach (var newViewModel in newViewModels)
            {
                flyEditingWidget.AddRouteToUi(newViewModel);
                newViewModel.PropertyChanged += (s, e) => { flyEditingWidget.Invalidate(); };
            }
            flyEditingWidget.Invalidate();
        }
    }

    private static void RemoveExistingRoutesFromControl(
       Map map,
       IEnumerable<RouteBaseViewModel> oldViewModels
    )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            flyEditingWidget.RemoveRouteFromUi(oldViewModels);
            foreach (var oldViewModel in oldViewModels)
            {
                oldViewModel.PropertyChanged -= (s, e) => { flyEditingWidget.Invalidate(); };
            }
            flyEditingWidget.Invalidate();
        }
    }

    private static void ResetRoutesOnControl(
        Map map
    )
    {
        var flyEditingWidget = map.Widgets.FirstOrDefault(t => t is FlyEditingWidget) as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            flyEditingWidget.ResetRoutesOnUi();
            flyEditingWidget.Invalidate();
        }
    }

    #endregion

    #region InfoRequestCommand
    public static ICommand? GetInfoRequestCommand(MapControl d)
    {
        return d.GetValue(InfoRequestCommandProperty);
    }
    public static void SetInfoRequestCommand(MapControl d, ICommand value)
    {
        d.SetValue(InfoRequestCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> InfoRequestCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "InfoRequestCommand", null);

    #endregion

    #region AddingNewMarkerCommand
    public static ICommand? GetAddingNewMarkerCommand(MapControl d)
    {
        return d.GetValue(AddingNewMarkerCommandProperty);
    }
    public static void SetAddingNewMarkerCommand(MapControl d, ICommand value)
    {
        d.SetValue(AddingNewMarkerCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> AddingNewMarkerCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "AddingNewMarkerCommand", null);

    #endregion

    #region StartMovingMarkerCommand
    public static ICommand? GetStartMovingMarkerCommand(MapControl d)
    {
        return d.GetValue(StartMovingMarkerCommandProperty);
    }
    public static void SetStartMovingMarkerCommand(MapControl d, ICommand value)
    {
        d.SetValue(StartMovingMarkerCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> StartMovingMarkerCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "StartMovingMarkerCommand", null);

    #endregion

    #region MovingMarkerCommand
    public static ICommand? GetMovingMarkerCommand(MapControl d)
    {
        return d.GetValue(MovingMarkerCommandProperty);
    }
    public static void SetMovingMarkerCommand(MapControl d, ICommand value)
    {
        d.SetValue(MovingMarkerCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> MovingMarkerCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "MovingMarkerCommand", null);

    #endregion

    #region EndMovingMarkerCommand
    public static ICommand? GetEndMovingMarkerCommand(MapControl d)
    {
        return d.GetValue(EndMovingMarkerCommandProperty);
    }
    public static void SetEndMovingMarkerCommand(MapControl d, ICommand value)
    {
        d.SetValue(EndMovingMarkerCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> EndMovingMarkerCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "EndMovingMarkerCommand", null);

    #endregion

    #region DeleteMarkerCommand
    public static ICommand? GetDeleteMarkerCommand(MapControl d)
    {
        return d.GetValue(DeleteMarkerCommandProperty);
    }
    public static void SetDeleteMarkerCommand(MapControl d, ICommand value)
    {
        d.SetValue(DeleteMarkerCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> DeleteMarkerCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "DeleteMarkerCommand", null);

    #endregion

    #region StartDrawingNewRouteCommand
    public static ICommand? GetStartDrawingNewRouteCommand(MapControl d)
    {
        return d.GetValue(StartDrawingNewRouteCommandProperty);
    }
    public static void SetStartDrawingNewRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(StartDrawingNewRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> StartDrawingNewRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "StartDrawingNewRouteCommand", null);

    #endregion

    #region HoveringNextPointOfRouteCommand
    public static ICommand? GetHoveringNextPointOfRouteCommand(MapControl d)
    {
        return d.GetValue(HoveringNextPointOfRouteCommandProperty);
    }
    public static void SetHoveringNextPointOfRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(HoveringNextPointOfRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> HoveringNextPointOfRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "HoveringNextPointOfRouteCommand", null);

    #endregion

    #region AddingNextWaypointToNewRouteCommand
    public static ICommand? GetAddingNextWaypointToNewRouteCommand(MapControl d)
    {
        return d.GetValue(AddingNextWaypointToNewRouteCommandProperty);
    }
    public static void SetAddingNextWaypointToNewRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(AddingNextWaypointToNewRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> AddingNextWaypointToNewRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "AddingNextWaypointToNewRouteCommand", null);

    #endregion

    #region EndDrawingNewRouteCommand
    public static ICommand? GetEndDrawingNewRouteCommand(MapControl d)
    {
        return d.GetValue(EndDrawingNewRouteCommandProperty);
    }
    public static void SetEndDrawingNewRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(EndDrawingNewRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> EndDrawingNewRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "EndDrawingNewRouteCommand", null);

    #endregion

    #region StartModifyingRouteCommand
    public static ICommand? GetStartModifyingRouteCommand(MapControl d)
    {
        return d.GetValue(StartModifyingRouteCommandProperty);
    }
    public static void SetStartModifyingRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(StartModifyingRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> StartModifyingRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "StartModifyingRouteCommand", null);

    #endregion

    #region ModifyingRouteCommand
    public static ICommand? GetModifyingRouteCommand(MapControl d)
    {
        return d.GetValue(ModifyingRouteCommandProperty);
    }
    public static void SetModifyingRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(ModifyingRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> ModifyingRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "ModifyingRouteCommand", null);

    #endregion

    #region EndModifyingRouteCommand
    public static ICommand? GetEndModifyingRouteCommand(MapControl d)
    {
        return d.GetValue(EndModifyingRouteCommandProperty);
    }
    public static void SetEndModifyingRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(EndModifyingRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> EndModifyingRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "EndModifyingRouteCommand", null);

    #endregion

    #region WaypointInsertedIntoRouteCommand
    public static ICommand? GetWaypointInsertedIntoRouteCommand(MapControl d)
    {
        return d.GetValue(WaypointInsertedIntoRouteCommandProperty);
    }
    public static void SetWaypointInsertedIntoRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(WaypointInsertedIntoRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> WaypointInsertedIntoRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "WaypointInsertedIntoRouteCommand", null);

    #endregion

    #region DeleteRouteWaypointCommand
    public static ICommand? GetDeleteRouteWaypointCommand(MapControl d)
    {
        return d.GetValue(DeleteRouteWaypointCommandProperty);
    }
    public static void SetDeleteRouteWaypointCommand(MapControl d, ICommand value)
    {
        d.SetValue(DeleteRouteWaypointCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> DeleteRouteWaypointCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "DeleteRouteWaypointCommand", null);

    #endregion

    #region DeleteRouteCommand
    public static ICommand? GetDeleteRouteCommand(MapControl d)
    {
        return d.GetValue(DeleteRouteCommandProperty);
    }
    public static void SetDeleteRouteCommand(MapControl d, ICommand value)
    {
        d.SetValue(DeleteRouteCommandProperty, value);
    }

    public static readonly AttachedProperty<ICommand?> DeleteRouteCommandProperty = AvaloniaProperty.RegisterAttached<MapControl, Interactive, ICommand?>(
        "DeleteRouteCommand", null);

    #endregion
}

