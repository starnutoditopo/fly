using Fly.Services;

namespace Fly.ViewModels;

public class PlaneViewModel : PlaneBaseViewModel
{
    public PlaneViewModel(
        IUnitOfMeasureService unitOfMeasureService,
        ISettingsService settingsService
        )
        : base(unitOfMeasureService, settingsService)
    {
    }
}
