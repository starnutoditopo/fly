using System.Threading;
using System.Threading.Tasks;
using Fly.Models;

namespace Fly.Services;

public interface IAirspaceInformationService
{
    Task<AirspacesInformationModel> GetAirspaceInformation(CoordinateModel coordinate, CancellationToken cancellationToken = default);
}
