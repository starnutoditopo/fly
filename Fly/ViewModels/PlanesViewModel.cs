using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fly.Models;
using Fly.Services;
using DialogHostAvalonia;
using ReactiveUI;
using Avalonia.Controls;

namespace Fly.ViewModels;

public class PlanesViewModel : ViewModelBase
{
    private static readonly object _lockObject = new object();
    private readonly IMapper _mapper;
    private readonly IUnitOfMeasureService _unitOfMeasureService;
    private readonly ISettingsService _settingsService;
    public PlanesViewModel(
        IMapper mapper,
        IUnitOfMeasureService unitOfMeasureService,
        ISettingsService settingsService
    )
    {
        _mapper = mapper;
        _unitOfMeasureService = unitOfMeasureService;
        _settingsService = settingsService;
        Planes = new ObservableCollection<PlaneBaseViewModel>();
    }

    public ObservableCollection<PlaneBaseViewModel> Planes { get; }

    private PlaneBaseViewModel? _selectedPlane;
    public PlaneBaseViewModel? SelectedPlane
    {
        get => _selectedPlane;
        set
        {
            SetProperty(ref _selectedPlane, value);
            this.RaisePropertyChanged(nameof(CanEdit));
            this.RaisePropertyChanged(nameof(CanRemove));
        }
    }

    public bool CanRemove => SelectedPlane != null;
    public async Task Remove()
    {
        PlaneBaseViewModel? plane = SelectedPlane;
        if (plane != null)
        {
            Planes.Remove(plane);
            SelectedPlane = null;
        }
        await Task.CompletedTask;
    }

    public bool CanEdit => SelectedPlane != null;

    public async Task Edit()
    {
        PlaneBaseViewModel? plane = SelectedPlane;
        if (plane != null)
        {
            var cloneModel = _mapper.Map<PlaneModel>(plane);
            var clone = _mapper.Map<PlaneBaseViewModel>(cloneModel);
            var viewModel = new EditPlaneViewModel(_unitOfMeasureService, clone);
            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditPlane.Title") as string);
            dialogViewModel.ContentViewModel = viewModel;
            var ok = await DialogHost.Show(dialogViewModel);
            if (true.Equals(ok))
            {
                var model = _mapper.Map<PlaneModel>(clone);
                _mapper.Map(model, plane);
            }
        }
    }

    internal int GetIdForNewPlane()
    {
        lock (_lockObject)
        {
            return Planes.Max(x => x.Id) + 1;
        }
    }

    public async Task Add()
    {
        PlaneBaseViewModel planeBaseViewModel = new PlaneViewModel(_unitOfMeasureService, _settingsService);
        planeBaseViewModel.Id = GetIdForNewPlane();
        Planes.Add(planeBaseViewModel);
        await Task.CompletedTask;
    }
}
