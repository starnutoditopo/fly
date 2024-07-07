namespace Fly.ViewModels;

public abstract class CoordinateBaseViewModel : ViewModelBase
{
    protected CoordinateBaseViewModel()
    {
    }

    private double _latitude;
    /// <summary>
    /// Gets or sets the latitude (in degrees, ranging from -90 to 90).
    /// </summary>
    /// <value>
    /// The latitude.
    /// </value>
    public virtual double Latitude
    {
        get=> _latitude;
        set => SetProperty(ref _latitude, value);
    }

    private double _longitude;
    /// <summary>
    /// Gets or sets the longitude (in degrees, ranging from -180 to 180).
    /// </summary>
    /// <value>
    /// The longitude.
    /// </value>
    public virtual double Longitude
    {
        get=> _longitude;
        set => SetProperty(ref _longitude, value);
    }

    private double? _elevation;
    /// <summary>
    /// Gets or sets the elevation, in meters.
    /// </summary>
    /// <value>
    /// The elevation, in meters.
    /// </value>
    /// <remarks>The elevation may be null, if not available. NaNs and Infinities are not supported (in compliance with json serialization).</remarks>
    public double? Elevation
    {
        get => _elevation;
        set => SetProperty(ref _elevation, value);
    }

    private string? _displayName;

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    /// <value>
    /// The display name.
    /// </value>
    public string? DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }
    private string? _city;

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    /// <value>
    /// The city.
    /// </value>
    public string? City
    {
        get => _city;
        set => SetProperty(ref _city, value);
    }
}