using Fly.Models.UnitsOfMeasure;
using System.Collections.Generic;

namespace Fly.Services;

public interface IUnitOfMeasureService
{
    IEnumerable<IUnitOfMeasure<Speed>> GetAvailableUnitsOfMeasureForSpeed();

    IEnumerable<IUnitOfMeasure<FuelConsumption>> GetAvailableUnitsOfMeasureForFuelConsumption();
}
