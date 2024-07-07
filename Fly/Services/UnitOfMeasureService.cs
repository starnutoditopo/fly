using Fly.Models.UnitsOfMeasure;
using System.Collections.Generic;

namespace Fly.Services;

public class UnitOfMeasureService : IUnitOfMeasureService
{
    public IEnumerable<IUnitOfMeasure<Speed>> GetAvailableUnitsOfMeasureForSpeed()
    {
        return new[] {
            UnitsOfMeasure.KilometerPerHour,
            UnitsOfMeasure.Knot
        };
    }

    public IEnumerable<IUnitOfMeasure<FuelConsumption>> GetAvailableUnitsOfMeasureForFuelConsumption()
    {
        return new[] {
            UnitsOfMeasure.LitersPerHour
        };
    }
}
