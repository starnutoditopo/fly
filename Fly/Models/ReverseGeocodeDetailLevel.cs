namespace Fly.Models;

// See: https://nominatim.org/release-docs/latest/api/Reverse/
public enum ReverseGeocodeDetailLevel
{
    Country = 3,
    State = 5,
    County = 8,
    City = 10,
    Suburb = 14,
    MajorStreets = 16,
    MajorAndMinorStreets = 17,
    Building = 18
}
