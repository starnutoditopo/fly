using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Widgets;
using Fly.StyleRenderers;
using Fly.Widgets;
using static Fly.Widgets.FlyEditingWidget;
//using MouseCoordinatesWidgetRenderer = Fly.Widgets.MouseCoordinatesWidgetRenderer;
using Fly.Services;
using Fly.AttachedProperties;
using MouseCoordinatesWidget = Fly.Widgets.MouseCoordinatesWidget;
using Avalonia.Xaml.Interactivity;
using Mapsui.UI.Avalonia;
using System;
using Avalonia.Preferences;
using System.Linq;

namespace Fly.Behaviors;

public class MapControlBehavior : Behavior<MapControl>
{
    protected override void OnAttached()
    {
        base.OnAttached();


        if (AssociatedObject.Renderer is Mapsui.Rendering.Skia.MapRenderer && !AssociatedObject.Renderer.StyleRenderers.ContainsKey(typeof(FlyStyle)))
        {
            AssociatedObject.Renderer.StyleRenderers.Add(typeof(FlyStyle), new FlyStyleRenderer());
        }

        var map = AssociatedObject.Map!;

        // TODO: inject
        ISettingsService settingsService = new SettingsService(new UnitOfMeasureService(), new Preferences());
        var homeCoordinates = settingsService.GetHome();
        var home = SphericalMercator
            .FromLonLat(
                homeCoordinates.Longitude,
                homeCoordinates.Latitude
            )
            .ToMPoint();

        map.Navigator.CenterOnAndZoomTo(home, 20);
        map.Navigator.PanLock = true;

        #region widgets
        // remove any existing widget (eg.: loggingWidget)
        //map.Widgets.Clear();

        // Scalebar widget
        map.Widgets.Enqueue(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(map) { TextAlignment = Alignment.Center, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Bottom });


        // Mouse Position Widget
        //AssociatedObject.Renderer.WidgetRenders.Add(typeof(MouseCoordinatesWidget), new MouseCoordinatesWidgetRenderer());
        map.Widgets.Enqueue(new MouseCoordinatesWidget());

        //// Center Position Widget
        //AssociatedObject.Renderer.WidgetRenders.Add(typeof(CenterCoordinatesWidget), new CenterCoordinatesWidgetRenderer());
        //map.Widgets.Enqueue(new CenterCoordinatesWidget(map));
        #endregion

        var rendererType = typeof(FlyEditingWidget);
        if (!AssociatedObject.Renderer.WidgetRenders.ContainsKey(rendererType))
        {
            AssociatedObject.Renderer.WidgetRenders.Add(rendererType, new FlyEditingWidgetRenderer());
        }
        FlyEditingWidget flyEditingWidget = new FlyEditingWidget(AssociatedObject);

        flyEditingWidget.InfoRequest += MyEditingWidget_InfoRequest;

        flyEditingWidget.AddingNewMarker += MyEditingWidget_AddingNewMarker;
        flyEditingWidget.StartMovingMarker += MyEditingWidget_StartMovingMarker;
        flyEditingWidget.MovingMarker += MyEditingWidget_MovingMarker;
        flyEditingWidget.EndMovingMarker += MyEditingWidget_EndMovingMarker;
        flyEditingWidget.DeleteMarker += MyEditingWidget_DeleteMarker;

        flyEditingWidget.StartDrawingNewRoute += MyEditingWidget_StartDrawingNewRoute;
        flyEditingWidget.HoveringNextPointOfRoute += MyEditingWidget_HoveringNextPointOfRoute;
        flyEditingWidget.AddingNextWaypointToNewRoute += MyEditingWidget_AddingNextWaypointToNewRoute;
        flyEditingWidget.EndDrawingNewRoute += MyEditingWidget_EndDrawingNewRoute;

        flyEditingWidget.StartModifyingRoute += MyEditingWidget_StartModifyingRoute;
        flyEditingWidget.ModifyingRoute += MyEditingWidget_ModifyingRoute;
        flyEditingWidget.EndModifyingRoute += MyEditingWidget_EndModifyingRoute;

        flyEditingWidget.WaypointInsertedIntoRoute += MyEditingWidget_WaypointInsertedIntoRoute;

        flyEditingWidget.DeleteRoute += MyEditingWidget_DeleteRoute;
        flyEditingWidget.DeleteRouteWaypoint += MyEditingWidget_DeleteRouteWaypoint;
        AssociatedObject.Map.Widgets.Enqueue(flyEditingWidget);
    }


    protected override void OnDetaching()
    {
        while (AssociatedObject.Map.Widgets.Any())
        {
            IWidget? widget;
            if (!AssociatedObject.Map.Widgets.TryDequeue(out widget))
            {
                throw new InvalidOperationException("Unable to dequeue widget");
            }
            if (widget is FlyEditingWidget flyEditingWidget)
            {
                flyEditingWidget.InfoRequest -= MyEditingWidget_InfoRequest;

                flyEditingWidget.AddingNewMarker -= MyEditingWidget_AddingNewMarker;
                flyEditingWidget.StartMovingMarker -= MyEditingWidget_StartMovingMarker;
                flyEditingWidget.MovingMarker -= MyEditingWidget_MovingMarker;
                flyEditingWidget.EndMovingMarker -= MyEditingWidget_EndMovingMarker;
                flyEditingWidget.DeleteMarker -= MyEditingWidget_DeleteMarker;

                flyEditingWidget.StartDrawingNewRoute -= MyEditingWidget_StartDrawingNewRoute;
                flyEditingWidget.HoveringNextPointOfRoute -= MyEditingWidget_HoveringNextPointOfRoute;
                flyEditingWidget.AddingNextWaypointToNewRoute -= MyEditingWidget_AddingNextWaypointToNewRoute;
                flyEditingWidget.EndDrawingNewRoute -= MyEditingWidget_EndDrawingNewRoute;

                flyEditingWidget.StartModifyingRoute -= MyEditingWidget_StartModifyingRoute;
                flyEditingWidget.ModifyingRoute -= MyEditingWidget_ModifyingRoute;
                flyEditingWidget.EndModifyingRoute -= MyEditingWidget_EndModifyingRoute;

                flyEditingWidget.WaypointInsertedIntoRoute -= MyEditingWidget_WaypointInsertedIntoRoute;

                flyEditingWidget.DeleteRoute -= MyEditingWidget_DeleteRoute;
                flyEditingWidget.DeleteRouteWaypoint -= MyEditingWidget_DeleteRouteWaypoint;
            }
        }
        base.OnDetaching();
    }

    #region InfoRequest
    private void MyEditingWidget_InfoRequest(object? sender, InfoRequestEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetInfoRequestCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.Coordinate))
                {
                    command.Execute(e.Coordinate);
                }
            }
        }
    }
    #endregion

    #region AddingNewMarker
    private void MyEditingWidget_AddingNewMarker(object? sender, AddingNewMarkerEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetAddingNewMarkerCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.Coordinate))
                {
                    command.Execute(e.Coordinate);
                }
            }
        }
    }
    #endregion

    #region StartMovingMarker
    private void MyEditingWidget_StartMovingMarker(object? sender, StartMovingMarkerEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetStartMovingMarkerCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.MouseCoordinate)))
                {
                    command.Execute((e.ViewModel, e.MouseCoordinate));
                }
            }
        }
    }
    #endregion

    #region MovingMarker
    private void MyEditingWidget_MovingMarker(object? sender, MovingMarkerEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetMovingMarkerCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.NewCoordinate)))
                {
                    command.Execute((e.ViewModel, e.NewCoordinate));
                }
            }
        }
    }
    #endregion

    #region EndMovingMarker
    private void MyEditingWidget_EndMovingMarker(object? sender, EndMovingMarkerEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetEndMovingMarkerCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.ViewModel))
                {
                    command.Execute(e.ViewModel);
                }
            }
        }
    }
    #endregion

    #region DeleteMarker
    private void MyEditingWidget_DeleteMarker(object? sender, DeleteMarkerEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetDeleteMarkerCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.ViewModel))
                {
                    command.Execute(e.ViewModel);
                }
            }
        }
    }
    #endregion

    #region StartDrawingNewRoute
    private void MyEditingWidget_StartDrawingNewRoute(object? sender, StartDrawingNewRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetStartDrawingNewRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.Coordinate))
                {
                    command.Execute(e.Coordinate);
                }
            }
        }
    }
    #endregion

    #region HoveringNextPointOfRoute
    private void MyEditingWidget_HoveringNextPointOfRoute(object? sender, HoveringNextPointOfRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetHoveringNextPointOfRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Coordinate)))
                {
                    command.Execute((e.ViewModel, e.Coordinate));
                }
            }
        }
    }
    #endregion

    #region AddingNextWaypointToNewRoute
    private void MyEditingWidget_AddingNextWaypointToNewRoute(object? sender, AddingNextWaypointToNewRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetAddingNextWaypointToNewRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Coordinate)))
                {
                    command.Execute((e.ViewModel, e.Coordinate));
                }
            }
        }
    }
    #endregion

    #region EndDrawingNewRoute
    private void MyEditingWidget_EndDrawingNewRoute(object? sender, EndDrawingNewRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetEndDrawingNewRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.ViewModel))
                {
                    command.Execute(e.ViewModel);
                }
            }
        }
    }
    #endregion

    #region StartModifyingRoute
    private void MyEditingWidget_StartModifyingRoute(object? sender, StartModifyingRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetStartModifyingRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Index, e.MouseCoordinate)))
                {
                    command.Execute((e.ViewModel, e.Index, e.MouseCoordinate));
                }
            }
        }
    }
    #endregion

    #region ModifyingRoute
    private void MyEditingWidget_ModifyingRoute(object? sender, ModifyingRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetModifyingRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Coordinate, e.Index)))
                {
                    command.Execute((e.ViewModel, e.Coordinate, e.Index));
                }
            }
        }
    }
    #endregion

    #region EndModifyingRoute
    private void MyEditingWidget_EndModifyingRoute(object? sender, EndModifyingRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetEndModifyingRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.ViewModel))
                {
                    command.Execute(e.ViewModel);
                }
            }
        }
    }
    #endregion

    #region WaypointInsertedIntoRoute

    private void MyEditingWidget_WaypointInsertedIntoRoute(object? sender, WaypointInsertedIntoRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetWaypointInsertedIntoRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Coordinate, e.Index)))
                {
                    command.Execute((e.ViewModel, e.Coordinate, e.Index));
                }
            }
        }
    }
    #endregion

    #region DeleteRouteWaypoint
    private void MyEditingWidget_DeleteRouteWaypoint(object? sender, DeleteRouteWaypointEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetDeleteRouteWaypointCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute((e.ViewModel, e.Index)))
                {
                    command.Execute((e.ViewModel, e.Index));
                }
            }
        }
    }
    #endregion



    #region DeleteRoute
    private void MyEditingWidget_DeleteRoute(object? sender, DeleteRouteEventArgs e)
    {
        FlyEditingWidget flyEditingWidget = sender as FlyEditingWidget;
        if (flyEditingWidget != null)
        {
            var command = MapControlAttachedProperties.GetDeleteRouteCommand(flyEditingWidget.MapControl as MapControl);
            if (command != null)
            {
                if (command.CanExecute(e.ViewModel))
                {
                    command.Execute(e.ViewModel);
                }
            }
        }
    }
    #endregion
}

