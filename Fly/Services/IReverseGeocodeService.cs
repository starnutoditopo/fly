using Fly.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fly.Services;

public interface IReverseGeocodeService
{
    Task<GeocodingInformation[]> GetGeocodeInformationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default);
}
