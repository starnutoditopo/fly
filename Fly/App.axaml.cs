using AutoMapper;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.Preferences;
using Fly.Comparers;
using Fly.Helpers;
using Fly.Models;
using Fly.Services;
using Fly.ViewModels;
using Fly.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Fly;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        SetLocale("en_US");
    }

    public static void SetLocale(string localeKey)
    {
        var app = Current as App;
        var targetLocale = app.Resources[localeKey] as ResourceDictionary;
        if (targetLocale == null || targetLocale == app._activeLocale)
        {
            return;
        }

        if (app._activeLocale != null)
        {
            app.Resources.MergedDictionaries.Remove(app._activeLocale);
        }

        app.Resources.MergedDictionaries.Add(targetLocale);
        app._activeLocale = targetLocale;
    }

    private ResourceDictionary _activeLocale = null;
    public override void OnFrameworkInitializationCompleted()
    {
        if (!Design.IsDesignMode)
        {
            Control mainView;
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                mainView = desktop.MainWindow;
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                singleViewPlatform.MainView = new MainView();
                mainView = singleViewPlatform.MainView;
            }
            else
            {
                Console.WriteLine($"ApplicationLifetime is: '{ApplicationLifetime}'");
                throw new NotImplementedException($"Method not implemented for applicationLifetime '{ApplicationLifetime}' (type {ApplicationLifetime?.GetType()})");
            }
            var topLevel = TopLevel.GetTopLevel(mainView)!;
            IClipboard? clipboard = topLevel.Clipboard;
            IStorageProvider storageProvider = topLevel.StorageProvider;

            #region setup DI (see: https://docs.avaloniaui.net/docs/guides/implementation-guides/how-to-implement-dependency-injection)
            // If you use CommunityToolkit, line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);

            // Register all the services needed for the application to run
            var collection = new ServiceCollection();
            collection.AddSingleton(storageProvider);
            if (clipboard != null)
            {
                collection.AddSingleton(clipboard);
            }
            collection.AddCommonServices();

            // Creates a ServiceProvider containing services from the provided IServiceCollection
            var serviceProvider = collection.BuildServiceProvider();
            #endregion

            var vm = serviceProvider.GetRequiredService<MainViewModel>();
            mainView.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public static class ServiceCollectionExtensions
{
    internal class MapperFactory
    {
        private static IMapper? mapper;
        private static readonly object _lockObject = new object();
        private readonly IUnitOfMeasureService _unitOfMeasureService;
        private readonly ISettingsService _settingsService;
        private readonly IReverseGeocodeService _reverseGeocodeService;
        private readonly IElevationService _elevationService;
        private readonly IAirspaceInformationService _airspaceInformationService;

        public MapperFactory(
            IUnitOfMeasureService unitOfMeasureService,
            ISettingsService settingsService,
            IReverseGeocodeService reverseGeocodeService,
            IElevationService elevationService,
            IAirspaceInformationService airspaceInformationService
        )
        {
            _unitOfMeasureService = unitOfMeasureService;
            _settingsService = settingsService;
            _reverseGeocodeService = reverseGeocodeService;
            _elevationService = elevationService;
            _airspaceInformationService = airspaceInformationService;
        }

        public IMapper GetMapper()
        {
            lock (_lockObject)
            {
                if (mapper == null)
                {
                    mapper = MappingHelper.ConfigureAutomapper(
                        _unitOfMeasureService,
                        _settingsService,
                        _reverseGeocodeService,
                        _elevationService,
                        _airspaceInformationService);
                }
            }
            return mapper;
        }
    }

    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<HttpClient, HttpClient>();
        //collection.AddSingleton<IEqualityComparer<CoordinateInformationModel>>(new CoordinateInformationModelEqualityComparer());
        collection.AddSingleton<IEqualityComparer<CoordinateModel>>(new CoordinateModelEqualityComparer());
        //collection.AddSingleton<ViewLocator>();


        //mauiAppBuilder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        collection.AddSingleton<MapperFactory, MapperFactory>();
        collection.AddSingleton(svc => svc.GetService<MapperFactory>()!.GetMapper());


        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<IUnitOfMeasureService, UnitOfMeasureService>();

        //collection.AddSingleton<IReverseGeocodeService, NominatimService>();
        //collection.AddSingleton<IElevationService, OpenElevationService>();


        collection.AddSingleton<OpenRouteService>();
        collection.AddSingleton<IReverseGeocodeService, OpenRouteService>(serviceProvider => serviceProvider.GetService<OpenRouteService>()!);
        collection.AddSingleton<IElevationService, OpenRouteService>(serviceProvider => serviceProvider.GetService<OpenRouteService>()!);

        collection.AddSingleton<IAirspaceInformationService, OpenAipService>();

        // Do not use IPreferences: see https://github.com/sandreas/Avalonia.Preferences for documentation
        collection.AddSingleton<Preferences>();

        #region register views and viewModels
        var types = Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t.FullName != null)
                    .Where(t => !t.IsAbstract)
                    .Where(t => !t.IsInterface);
        foreach (var type in types)
        {
            if (type.FullName!.StartsWith(nameof(Fly) + "." + nameof(ViewModels)) && type.Name.EndsWith("ViewModel"))
            {
                collection.AddTransient(type);
            }
            else if (type.FullName.StartsWith(nameof(Fly) + "." + nameof(Views)) && type.Name.EndsWith("View"))
            {
                collection.AddTransient(type);
            }
        }

        #endregion
    }
}