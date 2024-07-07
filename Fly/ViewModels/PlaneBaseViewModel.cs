using Fly.Models.UnitsOfMeasure;
using Fly.Services;

namespace Fly.ViewModels;

public abstract class PlaneBaseViewModel : ViewModelBase
{
    protected readonly IUnitOfMeasureService _unitOfMeasureService;

    protected PlaneBaseViewModel(
        IUnitOfMeasureService unitOfMeasureService,
        ISettingsService settingsService
    )
    {
        _unitOfMeasureService = unitOfMeasureService;
        _displayName = "New plane";
        _registrationNumber = string.Empty;

        _cruiseSpeedUnitOfMeasure = settingsService.GetFavouriteUnitOfMeasureForSpeed();
        _meanFuelConsumptionUnitOfMeasure = settingsService.GetFavouriteUnitOfMeasureForFuelConsumption();
    }

    private string _displayName;
    public string DisplayName
    {
        get => _displayName;
        set => SetProperty(ref _displayName, value);
    }

    private string _registrationNumber;
    public string RegistrationNumber
    {
        get => _registrationNumber;
        set => SetProperty(ref _registrationNumber, value);
    }

    private double _cruiseSpeed;

    /// <summary>
    /// Gets or sets the cruise speed.
    /// </summary>
    /// <value>
    /// The cruise speed.
    /// </value>
    public double CruiseSpeed
    {
        get => _cruiseSpeed;
        set => SetProperty(ref _cruiseSpeed, value);
    }

    private IUnitOfMeasure<Speed> _cruiseSpeedUnitOfMeasure;
    public IUnitOfMeasure<Speed> CruiseSpeedUnitOfMeasure
    {
        get => _cruiseSpeedUnitOfMeasure;
        set => SetProperty(ref _cruiseSpeedUnitOfMeasure, value);
    }

    private double _meanFuelConsumption;
    public double MeanFuelConsumption
    {
        get => _meanFuelConsumption;
        set => SetProperty(ref _meanFuelConsumption, value);
    }

    private IUnitOfMeasure<FuelConsumption> _meanFuelConsumptionUnitOfMeasure;
    public IUnitOfMeasure<FuelConsumption> MeanFuelConsumptionUnitOfMeasure
    {
        get => _meanFuelConsumptionUnitOfMeasure;
        set => SetProperty(ref _meanFuelConsumptionUnitOfMeasure, value);
    }


    private int _id;
    public int Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }
}
