using System;

namespace Fly.Models.UnitsOfMeasure;

public interface IUnitOfMeasure<TQuantity> :
    IUnitOfMeasure,
    IEquatable<IUnitOfMeasure<TQuantity>>
    where TQuantity : IQuantity
{
    string Code { get; }

    double ToStandardUnits(double value);
    double FromStandardUnits(double standardUnits);
}

public interface IUnitOfMeasure
{
    string DisplayName { get; }
}
