using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using System.Collections.Specialized;
using Fly.ViewModels;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Fly.Features;

public class RouteFeature : GeometryFeature
{
    public RouteFeature(RouteBaseViewModel viewModel)
    {
        ViewModel = viewModel;

        var worldPositions = viewModel
            .Coordinates
            .Select(coordinate =>
            {
                var worldPoint = SphericalMercator.FromLonLat(coordinate.Longitude, coordinate.Latitude);
                var worldPosition = worldPoint.ToCoordinate();
                return worldPosition;
            })
            .ToArray();

        Geometry = GetGeometry(worldPositions);

        AttachEvents();
    }

    private static Geometry GetGeometry(Coordinate[] worldPositions)
    {
        if (worldPositions.Length <= 0) // shouldn't happen
        {
            throw new InvalidOperationException("0-length route detected.");
        }
        else if (worldPositions.Length == 1)
        {
            var result = new Point(worldPositions[0]);
            return result;
        }
        else
        {
            var result = new LineString(worldPositions);
            return result;
        }
    }


    public RouteBaseViewModel ViewModel { get; }

    private void AttachEvents()
    {
        foreach (var coordinate in ViewModel.Coordinates)
        {
            coordinate.PropertyChanged += coordinate_PropertyChanged;
        }
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        ViewModel.Coordinates.CollectionChanged += Routes_CoordinatesCollectionChanged;
    }

    private void coordinate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        CoordinateBaseViewModel coordinateBaseViewModel = sender as CoordinateBaseViewModel;
        var index = ViewModel.Coordinates.IndexOf(coordinateBaseViewModel);
        if (Geometry is LineString lineString)
        {
            var lonLat = new MPoint(coordinateBaseViewModel.Longitude, coordinateBaseViewModel.Latitude);
            var worldPoint = SphericalMercator.FromLonLat(lonLat);
            lineString.Coordinates[index].SetXY(worldPoint);
        }
        else if (Geometry is Point point)
        {
            var lonLat = new MPoint(coordinateBaseViewModel.Longitude, coordinateBaseViewModel.Latitude);
            var worldPoint = SphericalMercator.FromLonLat(lonLat);
            point.Coordinate.SetXY(worldPoint);
        }
        else
        {
            throw new InvalidOperationException($"Geometry {Geometry} is of unhandled type.");
        }
    }

    internal void DetachEvents()
    {
        foreach (var coordinate in ViewModel.Coordinates)
        {
            coordinate.PropertyChanged -= coordinate_PropertyChanged;
        }
        ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        ViewModel.Coordinates.CollectionChanged -= Routes_CoordinatesCollectionChanged;
    }

    private void Routes_CoordinatesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            AddRoutePointsToUi(e.NewStartingIndex, e.NewItems.Cast<CoordinateBaseViewModel>().ToArray());
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            RemoveRoutePointsFromUi(e.OldStartingIndex, e.OldItems.Cast<CoordinateBaseViewModel>().ToArray());
        }
        else if (e.Action == NotifyCollectionChangedAction.Replace)
        {
            ReplaceRoutePointInUi(e.OldStartingIndex, e.OldItems.Cast<CoordinateBaseViewModel>().ToArray(), e.NewStartingIndex, e.NewItems.Cast<CoordinateBaseViewModel>().ToArray());
        }
        else
        {
            throw new NotImplementedException();
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RouteBaseViewModel.DisplayName))
        {
            // TODO...
        }
    }

    private void AddRoutePointsToUi(
       int newStartingIndex,
       CoordinateBaseViewModel[] newCoordinates
       )
    {
        if (Geometry is LineString lineString)
        {
            var newLineStringCoordinates = lineString
                .Coordinates
                .ToList();

            for (int i = 0; i < newCoordinates.Length; i++)
            {
                var newCoordinate = newCoordinates[i];
                var newLonLat = new MPoint(newCoordinate.Longitude, newCoordinate.Latitude);
                var newWorldPoint = SphericalMercator.FromLonLat(newLonLat);
                var newWorldPosition = newWorldPoint.ToCoordinate();

                newLineStringCoordinates.Insert(newStartingIndex + i, newWorldPosition);

                newCoordinate.PropertyChanged += coordinate_PropertyChanged;
            }

            var newGeometry = new LineString(newLineStringCoordinates.ToArray());
            Geometry = newGeometry;
        }
        else if (Geometry is Point point)
        {
            var newLineStringCoordinates = new List<Coordinate>() { point.Coordinate };

            for (int i = 0; i < newCoordinates.Length; i++)
            {
                var newCoordinate = newCoordinates[i];
                var newLonLat = new MPoint(newCoordinate.Longitude, newCoordinate.Latitude);
                var newWorldPoint = SphericalMercator.FromLonLat(newLonLat);
                var newWorldPosition = newWorldPoint.ToCoordinate();

                newLineStringCoordinates.Insert(newStartingIndex + i, newWorldPosition);

                newCoordinate.PropertyChanged += coordinate_PropertyChanged;
            }

            var newGeometry = new LineString(newLineStringCoordinates.ToArray());
            Geometry = newGeometry;
        }
    }

    private void RemoveRoutePointsFromUi(
      int oldStartingIndex,
      CoordinateBaseViewModel[] oldCoordinates
      )
    {
        foreach (var oldCoordinate in oldCoordinates)
        {
            oldCoordinate.PropertyChanged -= coordinate_PropertyChanged;
        }

        var lineString = Geometry as LineString;
        if (lineString != null)
        {
            var worldPositions = lineString
                .Coordinates
                .Take(oldStartingIndex)
                .Concat(lineString.Coordinates.Skip(oldStartingIndex + oldCoordinates.Length))
                .ToArray();

            Geometry = GetGeometry(worldPositions);
        }
    }

    private void ReplaceRoutePointInUi(
        int oldStartingIndex,
        CoordinateBaseViewModel[] oldCoordinates,
        int newStartingIndex,
        CoordinateBaseViewModel[] newCoordinates
    )
    {
        foreach (var oldCoordinate in oldCoordinates)
        {
            oldCoordinate.PropertyChanged -= coordinate_PropertyChanged;
        }

        var lineString = Geometry as LineString;
        if (lineString != null)
        {
            var newLineStringCoordinates = lineString
                .Coordinates
                .Take(oldStartingIndex)
                .Concat(lineString.Coordinates.Skip(oldStartingIndex + oldCoordinates.Length))
                .ToList();

            for (int i = 0; i < newCoordinates.Length; i++)
            {
                var newCoordinate = newCoordinates[i];
                var newLonLat = new MPoint(newCoordinate.Longitude, newCoordinate.Latitude);
                var newWorldPoint = SphericalMercator.FromLonLat(newLonLat);
                var newWorldPosition = newWorldPoint.ToCoordinate();

                newLineStringCoordinates.Insert(newStartingIndex + i, newWorldPosition);

                newCoordinate.PropertyChanged += coordinate_PropertyChanged;
            }

            var newGeometry = new LineString(newLineStringCoordinates.ToArray());
            Geometry = newGeometry;
        }
    }
}
