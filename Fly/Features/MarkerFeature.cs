using Mapsui;
using Mapsui.Nts;
using Mapsui.Nts.Extensions;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using Fly.ViewModels;

namespace Fly.Features;

public class MarkerFeature : GeometryFeature
{
    public MarkerFeature(MarkerBaseViewModel viewModel, Geometry? geometry)
    {
        ViewModel = viewModel;
        Geometry = geometry;

        AttachEvents();
    }

    public MarkerBaseViewModel ViewModel { get; }

    private void AttachEvents()
    {
        //ViewModel.PropertyChanged += MarkerViewModelBase_PropertyChanged;
        ViewModel.Coordinate.PropertyChanged += Coordinate_PropertyChanged;
    }
    internal void DetachEvents()
    {
        //ViewModel.PropertyChanged -= MarkerViewModelBase_PropertyChanged;
        ViewModel.Coordinate.PropertyChanged -= Coordinate_PropertyChanged;
    }

    private void Coordinate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var lonLat = new MPoint(ViewModel.Coordinate.Longitude, ViewModel.Coordinate.Latitude);
        var worldPoint = SphericalMercator.FromLonLat(lonLat);

        Geometry?.Coordinate.SetXY(worldPoint);
    }

    //private void MarkerViewModelBase_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(MarkerBaseViewModel.DisplayName))
    //    {
    //        // TODO...
    //    }
    //}
}
