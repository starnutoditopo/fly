using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Fly.Services;
using ReactiveUI;

namespace Fly.ViewModels;

public class LayersViewModel : ViewModelBase
{
    private readonly ISettingsService _settingsService;
    public LayersViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;

        Layers = new ObservableCollection<LayerBaseViewModel>();

        var openStreetMap_urlformatter = _settingsService.GetOpenStreetMap_Map_Urlformatter();
        var openStreetMap_userAgent = _settingsService.GetOpenStreetMap_UserAgent();
        var osmLayerType = new LayerType(
            "Open street map",
            () =>
            {
                var layer = new OpenStreetMapLayerViewModel(openStreetMap_urlformatter, openStreetMap_userAgent)
                {
                    IsEnabled = true,
                    Opacity = 1
                };
                return layer;
            },
            () => !string.IsNullOrWhiteSpace(openStreetMap_urlformatter)
                    && !string.IsNullOrWhiteSpace(openStreetMap_userAgent)
        );

        var mapTilerSatellite_apiKey = _settingsService.GetMapTilerSatellite_ApiKey();
        var mapTilerSatellite_urlformatter = _settingsService.GetMapTilerSatellite_Map_Urlformatter();
        var mapTilerSatellite_userAgent = _settingsService.GetMapTilerSatellite_UserAgent();

        var mapTilerSatellite = new LayerType(
            "MapTiler satellite",
            () =>
            {
                var layer = new MapTilerSatelliteLayerViewModel(
                    mapTilerSatellite_apiKey,
                    mapTilerSatellite_urlformatter,
                    mapTilerSatellite_userAgent
                    )
                {
                    IsEnabled = true,
                    Opacity = 1
                };
                return layer;
            },
            () => !string.IsNullOrWhiteSpace(mapTilerSatellite_apiKey)
                    && !string.IsNullOrWhiteSpace(mapTilerSatellite_urlformatter)
                    && !string.IsNullOrWhiteSpace(mapTilerSatellite_userAgent)
        );

        var openAip_apiKey = _settingsService.GetOpenAIP_ApiKey();
        var openAip_urlformatter = _settingsService.GetOpenAIP_Map_Urlformatter();
        var openAip_serversList = _settingsService.GetOpenAIP_Map_ServersList();
        var openAip_userAgent = _settingsService.GetOpenAIP_UserAgent();

        var openAipLayerType = new LayerType(
            "Open AIP",
            () =>
            {
                var layer = new OpenAIPLayerViewModel(
                    openAip_apiKey,
                    openAip_urlformatter,
                    openAip_serversList,
                    openAip_userAgent
                )
                {
                    IsEnabled = true,
                    Opacity = 1
                };
                return layer;
            },
            () => !string.IsNullOrWhiteSpace(openAip_apiKey)
                    && !string.IsNullOrWhiteSpace(openAip_urlformatter)
                    && !string.IsNullOrWhiteSpace(openAip_userAgent)
                    && openAip_serversList != null
                    && openAip_serversList.Any()
        );
        var featuresLayerType = new LayerType(
            "Features",
            () =>
            {
                var layer = new FeaturesLayerViewModel()
                {
                    IsEnabled = true,
                    Opacity = 1
                };
                return layer;
            },
            () =>
            {
                // Only one feature layer is allowed
                return !Layers.OfType< FeaturesLayerViewModel >().Any();
            }
        );
        AvailableLayerTypes = new List<LayerType>()
        {
            osmLayerType,
            mapTilerSatellite,
            openAipLayerType,
            featuresLayerType
        }.Where(t => t.CanCreateLayer)
        .ToList();

        Task.Run(() =>
        {
            foreach(var item in AvailableLayerTypes)
            {
                Layers.Add(item.CreateLayer());
            }
        });
    }   

    public async Task AddNewLayer(LayerType layerType)
    {
        if (layerType != null)
        {
            var layer = layerType.CreateLayer();
            Layers.Add(layer);
        }

        await Task.CompletedTask;
    }

    public List<LayerType> AvailableLayerTypes { get; }
    public ObservableCollection<LayerBaseViewModel> Layers { get; }

    public bool CanRemove => SelectedLayer != null;    

    public async Task Remove()
    {
        LayerBaseViewModel? layer = SelectedLayer;
        if (layer != null)
        {
            Layers.Remove(layer);
            this.RaisePropertyChanged(nameof(SelectedLayer));
        }
        await Task.CompletedTask;
    }
    public bool CanMoveUp => SelectedLayer != null && Layers.IndexOf(SelectedLayer) > 0;
    
    public async Task MoveUp()
    {
        LayerBaseViewModel? layer = SelectedLayer;
        if (layer != null)
        {
            int currentIndex = Layers.IndexOf(layer);
            if (currentIndex > 0)
            {
                Layers.Move(currentIndex, currentIndex - 1);
            }
        }
        await Task.CompletedTask;
    }
    public bool CanMoveDown => SelectedLayer != null && Layers.IndexOf(SelectedLayer) < Layers.Count - 1;
    
    public async Task MoveDown()
    {
        LayerBaseViewModel? layer = SelectedLayer;
        if (layer != null)
        {
            int currentIndex = Layers.IndexOf(layer);
            if (currentIndex < Layers.Count - 1)
            {
                Layers.Move(currentIndex, currentIndex + 1);
            }
        }
        await Task.CompletedTask;
    }

    private LayerBaseViewModel? _selectedLayer;
    public LayerBaseViewModel? SelectedLayer
    {
        get=> _selectedLayer;
        set
        {
            SetProperty(ref _selectedLayer, value);
            this.RaisePropertyChanged(nameof(CanRemove));
            this.RaisePropertyChanged(nameof(CanMoveUp));
            this.RaisePropertyChanged(nameof(CanMoveDown));
        }
    }    
}