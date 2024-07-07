using AutoMapper;
using Avalonia.Platform.Storage;
using Fly.Models;
using Fly.Services;
using DialogHostAvalonia;
using System;
using System.IO.Compression;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Controls;
using ReactiveUI;
using System.Linq;

namespace Fly.ViewModels;

public class MainViewModel : ViewModelBase
{
    private const string DOCUMENT_ENTRY_NAME = "document.json"; // Name of json document entry within a .fly file

    private readonly ISettingsService _settingsService;
    private readonly IStorageProvider _storageProvider;
    private readonly IMapper _mapper;

    public MainViewModel(
        IMapper mapper,
        IStorageProvider storageProvider,
        ISettingsService settingsService,
        AboutViewModel aboutViewModel,
        EditSettingsViewModel EditSettingsViewModel,
        DocumentViewModel documentViewModel,
        ConversionsViewModel conversionsViewModel
    )
    {
        _mapper = mapper;
        _storageProvider = storageProvider;
        _aboutViewModel = aboutViewModel;
        _documentViewModel = documentViewModel;
        _editSettingsViewModel = EditSettingsViewModel;
        _settingsService = settingsService;
        _conversionsViewModel = conversionsViewModel;
        _documentInformation = null;

        ActiveViewModel = _documentViewModel;

        this.PropertyChanged += MainViewModel_PropertyChanged;
    }

    private void MainViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentInformation))
        {
            this.RaisePropertyChanged(nameof(CanSaveDocument));
        }
    }

    private readonly AboutViewModel _aboutViewModel;
    private readonly DocumentViewModel _documentViewModel;
    private readonly EditSettingsViewModel _editSettingsViewModel;
    private readonly ConversionsViewModel _conversionsViewModel;

    public async Task Loaded()
    {
        if (_settingsService.GetAutomaticallyOpenLastDocument())
        {
            string? bookmark = _settingsService.GetLastOpenedDocument();
            if (!string.IsNullOrWhiteSpace(bookmark))
            {
                IStorageFile? storageFile = await _storageProvider.OpenFileBookmarkAsync(bookmark);
                if (storageFile != null)
                {
                    await LoadFromFile(storageFile);
                }
            }
        }
    }

    public async Task ShowAbout()
    {
        var dialogViewModel = new DialogViewModel();
        dialogViewModel.ContentViewModel = _aboutViewModel;
        await DialogHost.Show(dialogViewModel);
    }

    public async Task ShowConvertUtility()
    {
        var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.ConversionUtility.Title") as string);
        dialogViewModel.ContentViewModel = _conversionsViewModel;
        await DialogHost.Show(dialogViewModel);
    }
    private ViewModelBase? _activeViewModel;
    public ViewModelBase? ActiveViewModel
    {
        get => _activeViewModel;
        set => SetProperty(ref _activeViewModel, value);
    }

    #region DocumentName
    private DocumentInformation? _documentInformation;
    public DocumentInformation? DocumentInformation
    {
        get => _documentInformation;
        set => SetProperty(ref _documentInformation, value);
    }
    #endregion

    public async Task NewDocument()
    {
        var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.ConfirmNewFileDialog.Title") as string);
        dialogViewModel.ContentViewModel = new MessageDialogViewModel(Avalonia.Application.Current.FindResource("Text.ConfirmNewFileDialog.Message") as string);
        var ok = await DialogHost.Show(dialogViewModel);
        if (true.Equals(ok))
        {
            _documentViewModel.Markers.Markers.Clear();
            _documentViewModel.Routes.Routes.Clear();
            _documentViewModel.Planes.Planes.Clear();
            _documentViewModel.FlightPlans.FlightPlans.Clear();
            DocumentInformation = null;
            _settingsService.SetLastOpenedDocument(null);
        }
    }

    public async Task OpenDocument()
    {
        var filePickerOpenOptions = new FilePickerOpenOptions()
        {
            Title = Avalonia.Application.Current!.FindResource("Text.OpenFileDialog.Title") as string,
            AllowMultiple = false
        };

        var storageFiles = await _storageProvider.OpenFilePickerAsync(filePickerOpenOptions);
        if (storageFiles != null)
        {
            IStorageFile? storageFile = storageFiles.FirstOrDefault();
            if (storageFile != null)
            {
                await LoadFromFile(storageFile);
            }
        }
    }

    public async Task LoadFromFile(IStorageFile fileResult)
    {
        using (Stream stream = await fileResult.OpenReadAsync())
        using (var archive = new ZipArchive(stream, ZipArchiveMode.Read, true))
        {
            var entry = archive.GetEntry(DOCUMENT_ENTRY_NAME);
            if (entry == null)
            {
                throw new InvalidOperationException("Invalid file format");
            }
            using (var entryStream = entry.Open())
            {
                DocumentModel? nullableModel = await JsonSerializer.DeserializeAsync<DocumentModel>(entryStream, Helpers.JsonSerializationHelper.JsonSerializerOptions);
                if (nullableModel == null)
                {
                    throw new InvalidOperationException("Invalid file format");
                }
                DocumentModel model = nullableModel;
                try
                {
                    _mapper.Map(model, _documentViewModel);
                }
                catch (Exception e)
                {
                    throw;
                }

                if (fileResult.CanBookmark)
                {
                    string? bookmark = await fileResult.SaveBookmarkAsync();
                    if (bookmark != null)
                    {
                        DocumentInformation = new DocumentInformation(fileResult.Name, bookmark);
                        _settingsService.SetLastOpenedDocument(bookmark);
                    }
                }
            }
        }
    }

    //public Task HandleAsync(ImportDocumentMessage message, CancellationToken cancellationToken)
    //{
    //    OpenFileDialog openFileDialog = new OpenFileDialog();
    //    openFileDialog.Filter = GPX_FILE_FILTER;
    //    if (openFileDialog.ShowDialog() == true)
    //    {
    //        GpsData data;
    //        using (Stream stream = openFileDialog.OpenFile())
    //        {
    //            data = GpsData.Parse(stream);
    //        }
    //        //var features = new List<IFeature>();

    //        #region load routes
    //        List<RouteBaseViewModel> routes = new List<RouteBaseViewModel>();
    //        foreach (var route in data.Routes)
    //        {
    //            RouteViewModel routeViewModel = new RouteViewModel()
    //            {
    //                Id = routes.GenerateNextId(r => r.Id),
    //                DisplayName = route.Metadata["name"]
    //            };
    //            foreach (var waypoint in route.Waypoints)
    //            {
    //                routeViewModel.Coordinates.Add(new CoordinateViewModel(_elevationService, _reverseGeocodeService)
    //                {
    //                    Latitude = waypoint.Coordinate.Latitude,
    //                    Longitude = waypoint.Coordinate.Longitude
    //                });


    //            }
    //            routes.Add(routeViewModel);
    //        }

    //        this.Routes.Routes.Clear();
    //        foreach (var route in routes)
    //        {
    //            this.Routes.Routes.Add(route);
    //        }
    //        #endregion
    //        #region load tracks
    //        foreach (var track in data.Tracks)
    //        {
    //            //GeometryFeature? geometryFeature = new GeometryFeature();
    //            //foreach (var segment in track.Segments)
    //            //{
    //            //    var linestring = segment.ToLineString();
    //            //    var points = linestring
    //            //        .Coordinates
    //            //        .Select(c => ToGeometryCoordinate(c))
    //            //        .ToArray();
    //            //    geometryFeature.Geometry = new LineString(points);
    //            //}
    //            //features.Add(geometryFeature);
    //        }
    //        #endregion
    //        #region load markers
    //        List<MarkerBaseViewModel> markers = new List<MarkerBaseViewModel>();
    //        foreach (var waypoint in data.Waypoints)
    //        {
    //            var coordinate = new CoordinateModel(waypoint.Coordinate.Latitude, waypoint.Coordinate.Longitude);
    //            var marker = new MarkerViewModel(_reverseGeocodeService, _elevationService, coordinate)
    //            {
    //                DisplayName = waypoint.Name
    //            };
    //            markers.Add(marker);
    //        }

    //        this.Markers.Markers.Clear();
    //        foreach (var marker in markers)
    //        {
    //            this.Markers.Markers.Add(marker);
    //        }
    //        #endregion

    //        //    MapControl mapControl = GetMapControl(view);
    //        //    WritableLayer writableLayer = GetWritableLayer(mapControl);
    //        //    writableLayer.Clear();
    //        //    writableLayer.AddRange(features);
    //        //    mapControl.RefreshGraphics();
    //    }
    //    return Task.CompletedTask;
    //}

    public bool CanSaveDocument => DocumentInformation != null;

    public async Task SaveDocument()
    {
        if (CanSaveDocument)
        {
            IStorageFile? storageFile = await _storageProvider.OpenFileBookmarkAsync(DocumentInformation.Bookmark);
            await Save(storageFile!);
        }
    }

    public async Task SaveDocumentAs()
    {
        IStorageFolder? suggestedStartLocation = null;
        string suggestedFileName = "Document.fly";

        #region retrieve suggestedStartLocation and suggestedFileName
        if (DocumentInformation != null)
        {
            suggestedFileName = DocumentInformation.DisplayName;

            if (DocumentInformation.Bookmark != null)
            {
                var storageBookmarkFile = await _storageProvider.OpenFileBookmarkAsync(DocumentInformation.Bookmark);
                if (storageBookmarkFile != null)
                {
                    suggestedStartLocation = await storageBookmarkFile.GetParentAsync();
                }
            }
        }
        #endregion

        FilePickerSaveOptions filePickerSaveOptions = new FilePickerSaveOptions()
        {
            Title = Avalonia.Application.Current.FindResource("Text.SaveFileDialog.Title") as string,
            DefaultExtension = ".fly",
            ShowOverwritePrompt = true,
            SuggestedFileName = suggestedFileName,
            SuggestedStartLocation = suggestedStartLocation

            // TODO: set FileTypeChoices
            //FileTypeChoices =
        };

        IStorageFile? storageFile = await _storageProvider.SaveFilePickerAsync(filePickerSaveOptions);
        if (storageFile != null)
        {
            string? bookmark;
            if (storageFile.CanBookmark)
            {
                bookmark = await storageFile.SaveBookmarkAsync();
                if (bookmark != null)
                {
                    await Save(storageFile);
                    DocumentInformation newDocumentInformation = new DocumentInformation(storageFile.Name, bookmark);
                    DocumentInformation = newDocumentInformation;
                    _settingsService.SetLastOpenedDocument(bookmark);
                    return;
                }
            }
        }
    }

    private async Task Save(IStorageFile? storageFile)
    {
        if (storageFile == null)
        {
            throw new InvalidOperationException("Unable to save the file.");
        }

        try
        {
            using (Stream stream = await storageFile.OpenWriteAsync())
            {
                using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
                {
                    var entry = archive.CreateEntry(DOCUMENT_ENTRY_NAME);
                    using (var entryStream = entry.Open())
                    {
                        DocumentModel model;
                        try
                        {
                            model = _mapper.Map<DocumentModel>(this._documentViewModel);
                        }
                        catch (Exception ex)
                        {
                            int i = 0;
                            throw;
                        }
                        await JsonSerializer.SerializeAsync(entryStream, model, Helpers.JsonSerializationHelper.JsonSerializerOptions);
                    }
                }
            }

            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.ApplicationNotificationInformation.Title") as string);
            dialogViewModel.ContentViewModel = new MessageDialogViewModel($"File {storageFile.Name} saved.");
            await DialogHost.Show(dialogViewModel);

        }
        catch (Exception ex)
        {
            var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.ApplicationNotificationError.Title") as string);
            dialogViewModel.ContentViewModel = new MessageDialogViewModel(ex);
            await DialogHost.Show(dialogViewModel);
        }
    }

    public async Task ShowSettings()
    {
        var dialogViewModel = new DialogViewModel(Avalonia.Application.Current.FindResource("Text.EditSettings.Title") as string);
        dialogViewModel.ContentViewModel = _editSettingsViewModel;
        await DialogHost.Show(dialogViewModel);
    }

    public async Task ShowDocument()
    {
        ActiveViewModel = _documentViewModel;
        await Task.CompletedTask;
    }
    public async Task ShowPlanes()
    {
        ActiveViewModel = _documentViewModel.Planes;
        await Task.CompletedTask;
    }
    public async Task ShowFlightPlans()
    {
        ActiveViewModel = _documentViewModel.FlightPlans;
        await Task.CompletedTask;
    }
}
