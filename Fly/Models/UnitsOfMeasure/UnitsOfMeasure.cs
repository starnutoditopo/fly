using System;

namespace Fly.Models.UnitsOfMeasure;

public static class UnitsOfMeasure
{
    private class UnitOfMeasure<TQuantity> :
        IUnitOfMeasure<TQuantity>
        where TQuantity : IQuantity
    {
        private readonly Func<double, double> _toStandardUnits;
        private readonly Func<double, double> _fromStandardUnits;
        public UnitOfMeasure(string code, string displayName, string description, Func<double, double> toStandardUnits, Func<double, double> fromStandardUnits)
        {
            Code = code;
            DisplayName = displayName;
            Description = description;
            _toStandardUnits = toStandardUnits;
            _fromStandardUnits = fromStandardUnits;
        }
        public virtual string Code { get; } = null!;

        public virtual string DisplayName { get; } = null!;
        public virtual string Description { get; } = null!;

        public bool Equals(IUnitOfMeasure<TQuantity>? other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return Code.Equals(other.Code);
        }
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as IUnitOfMeasure<TQuantity>);
        }

        public double ToStandardUnits(double value)
        {
            return _toStandardUnits(value);
        }

        public double FromStandardUnits(double standardUnits)
        {
            return _fromStandardUnits(standardUnits);
        }

        public static bool operator ==(UnitOfMeasure<TQuantity> obj1, IUnitOfMeasure<TQuantity> obj2)
        {
            return obj1.Equals((object)obj2);
        }
        public static bool operator !=(UnitOfMeasure<TQuantity> obj1, IUnitOfMeasure<TQuantity> obj2)
        {
            return !obj1.Equals((object)obj2);
        }
        public static bool operator ==(IUnitOfMeasure<TQuantity> obj1, UnitOfMeasure<TQuantity> obj2)
        {
            return obj1.Equals((object)obj2);
        }
        public static bool operator !=(IUnitOfMeasure<TQuantity> obj1, UnitOfMeasure<TQuantity> obj2)
        {
            return !obj1.Equals((object)obj2);
        }
    }

    private static IUnitOfMeasure<Length> _kilometer = new UnitOfMeasure<Length>("Km", "Km", "Kilometers", x => x * 1000, x => x / 1000);
    private static IUnitOfMeasure<Length> _meter = new UnitOfMeasure<Length>("m", "m", "meters", x => x, x => x);
    private static IUnitOfMeasure<Length> _foot = new UnitOfMeasure<Length>("ft", "ft", "feet", x => x * 0.3048, x => x / 0.3048);
    private static IUnitOfMeasure<Length> _mile = new UnitOfMeasure<Length>("NM", "NM", "nautical miles", x => x * 1852, x => x / 1852);
    private static IUnitOfMeasure<Speed> _kilometerPerHour = new UnitOfMeasure<Speed>("Km/h", "Km/h", "Kilometers per hour", x => x / 3.6, x => x * 3.6);
    private static IUnitOfMeasure<Speed> _meterPerSecond = new UnitOfMeasure<Speed>("m/s", "m/s", "meters per second", x => x, x => x);
    private static IUnitOfMeasure<Speed> _knot = new UnitOfMeasure<Speed>("kt", "kt", "Knots", x => x / 1.944 , x => x * 1.944);
    private static IUnitOfMeasure<FuelConsumption> _litersPerHour = new UnitOfMeasure<FuelConsumption>("l/h", "l/h", "liters per hour", x => x / 3600, x => x * 3600);
    private static IUnitOfMeasure<FuelConsumption> _litersPerSecond = new UnitOfMeasure<FuelConsumption>("l/s", "l/s", "liters per second", x => x, x => x);

    private static double GallonsToLiters(double usGallon)
    {
        return usGallon * 3.785411784 / 1000;
    }
    private static double LitersToGallons(double liters)
    {
        return liters * 1000 / 3.785411784;
    }

    private static IUnitOfMeasure<FuelConsumption> _gallonsPerHour = new UnitOfMeasure<FuelConsumption>("US gal/h", "US gal/h", "US gallons per hour", x => GallonsToLiters(x) / 3600 * 1000, x => LitersToGallons(x) * 3600 / 1000);
    private static IUnitOfMeasure<FuelConsumption> _gallonsPerSecond = new UnitOfMeasure<FuelConsumption>("US gal/s", "US gal/s", "US gallons per second", x => GallonsToLiters(x) * 1000, x => LitersToGallons(x) / 1000);
    private static IUnitOfMeasure<Volume> _liter = new UnitOfMeasure<Volume>("l", "l", "liters", x => x / 1000, x => x * 1000);
    private static IUnitOfMeasure<Volume> _gallon = new UnitOfMeasure<Volume>("US gal", "US gal", "US gallons", GallonsToLiters, LitersToGallons);

    public static IUnitOfMeasure<Volume> Gallon => _gallon;
    public static IUnitOfMeasure<Volume> Liter => _liter;
    public static IUnitOfMeasure<Length> Kilometer => _kilometer;
    public static IUnitOfMeasure<Length> Meter => _meter;
    public static IUnitOfMeasure<Length> Foot => _foot;
    public static IUnitOfMeasure<Length> Mile => _mile;
    public static IUnitOfMeasure<Speed> KilometerPerHour => _kilometerPerHour;
    public static IUnitOfMeasure<Speed> MeterPerSecond => _meterPerSecond;
    
    /// <summary>
    /// Define the knot (1 kt = 1.852 Km/h).
    /// </summary>
    /// <value>
    /// The knot unit of measure.
    /// </value>
    public static IUnitOfMeasure<Speed> Knot => _knot;
    public static IUnitOfMeasure<FuelConsumption> LitersPerHour => _litersPerHour;
    public static IUnitOfMeasure<FuelConsumption> LitersPerSecond => _litersPerSecond;
    public static IUnitOfMeasure<FuelConsumption> GallonsPerHour => _gallonsPerHour;
    public static IUnitOfMeasure<FuelConsumption> GallonsPerSecond => _gallonsPerSecond;
}
