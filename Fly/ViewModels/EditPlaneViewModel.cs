using Fly.Models.UnitsOfMeasure;
using Fly.Services;
using System.Collections.Generic;

namespace Fly.ViewModels;

public class EditPlaneViewModel : ViewModelBase
{
    public EditPlaneViewModel(
        IUnitOfMeasureService unitOfMeasureService,
        PlaneBaseViewModel plane
        )
    {
        Plane = plane;
        AvailableUnitsOfMeasureForSpeed = unitOfMeasureService.GetAvailableUnitsOfMeasureForSpeed();
        AvailableUnitsOfMeasureForFuelConsumption = unitOfMeasureService.GetAvailableUnitsOfMeasureForFuelConsumption();
    }

    public PlaneBaseViewModel Plane { get; }

    public IEnumerable<IUnitOfMeasure<Speed>> AvailableUnitsOfMeasureForSpeed { get; }
    public IEnumerable<IUnitOfMeasure<FuelConsumption>> AvailableUnitsOfMeasureForFuelConsumption { get; }
}
