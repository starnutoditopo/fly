using System.Collections.ObjectModel;
using Fly.Extensions;
using ReactiveUI;

namespace Fly.ViewModels;

public abstract class RouteBaseViewModel : ViewModelBase
{
    protected RouteBaseViewModel()
    {
        var coordinates = new ObservableCollection<CoordinateBaseViewModel>();
        coordinates.CollectionChanged += Coordinates_CollectionChanged;
        Coordinates = coordinates;
        _displayName = Constants.ROUTE_DEFAULT_NAME;
        _isVisible = true;
    }

    private void Coordinates_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add
            || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace
            )
        {
            if (e.NewItems != null)
            {
                foreach (CoordinateBaseViewModel coordinate in e.NewItems)
                {
                    coordinate.PropertyChanged += Coordinate_PropertyChanged;
                }
            }
        }
        if (
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove
            || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace
            )
        {
            if (e.OldItems != null) // may happen if Action is Reset
            {
                foreach (CoordinateBaseViewModel coordinate in e.OldItems)
                {
                    coordinate.PropertyChanged -= Coordinate_PropertyChanged;
                }
            }
        }
        this.RaisePropertyChanged(nameof(RouteLength));
    }

    private void Coordinate_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(RouteLength));
    }

    public virtual ObservableCollection<CoordinateBaseViewModel> Coordinates { get; }

    private string _displayName;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    /// <summary>
    /// Gets the length of the route, in Km.
    /// </summary>
    /// <value>
    /// The length of the route, in Km.
    /// </value>
    public double RouteLength
    {
        get
        {
            double result = 0;
            for (int i = 1; i < Coordinates.Count; i++)
            {
                var previousCoordinate = Coordinates[i - 1];
                var currentCoordinate = Coordinates[i];
                double rhumbDistance = GeographyExtensions.GetRhumbDistance(previousCoordinate.Longitude, previousCoordinate.Latitude, currentCoordinate.Longitude, currentCoordinate.Latitude);
                result += rhumbDistance;
            }
            return result / 1000;
        }
    }

    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }
}
