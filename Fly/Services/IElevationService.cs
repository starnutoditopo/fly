using Fly.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Fly.Services;

public interface IElevationService
{
    Task<double?[]> GetElevationForCoordinates(CoordinateModel[] coordinates, CancellationToken cancellationToken = default);
}
