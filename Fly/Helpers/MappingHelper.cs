using AutoMapper;
using AutoMapper.EquivalencyExpression;
using Fly.Models;
using Fly.Services;
using Fly.ViewModels;
using System.Linq;

namespace Fly.Helpers
{
    internal class MappingHelper
    {
        public static IMapper ConfigureAutomapper(
            IUnitOfMeasureService _unitOfMeasureService,
             ISettingsService _settingsService,
             IReverseGeocodeService _reverseGeocodeService,
             IElevationService _elevationService,
             IAirspaceInformationService _airspaceInformationService
            )
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddCollectionMappers();

                #region vm -> m
                //cfg.CreateMap<CoordinateBaseViewModel, CoordinateModel>();
                cfg.CreateMap<CoordinateBaseViewModel, FullCoordinateInformationModel>()
                    .ForPath(m => m.Coordinate.Latitude, opt => opt.MapFrom(vm => vm.Latitude))
                    .ForPath(m => m.Coordinate.Longitude, opt => opt.MapFrom(vm => vm.Longitude))
                    ;
                cfg.CreateMap<MarkerBaseViewModel, MarkerModel>()
                    .ForPath(vm => vm.FullCoordinateInformationModel.Coordinate.Latitude, opt => opt.MapFrom(m => m.Coordinate.Latitude))
                    .ForPath(vm => vm.FullCoordinateInformationModel.Coordinate.Longitude, opt => opt.MapFrom(m => m.Coordinate.Longitude))
                    .ForPath(vm => vm.FullCoordinateInformationModel.DisplayName, opt => opt.MapFrom(m => m.Coordinate.DisplayName))
                    .ForPath(vm => vm.FullCoordinateInformationModel.Elevation, opt => opt.MapFrom(m => m.Coordinate.Elevation))
                    .ForPath(vm => vm.FullCoordinateInformationModel.City, opt => opt.MapFrom(m => m.Coordinate.City))
                    ;
                cfg.CreateMap<RouteBaseViewModel, RouteModel>();
                cfg.CreateMap<PlaneBaseViewModel, PlaneModel>();
                cfg.CreateMap<FlightPlanBaseViewModel, FlightPlanModel>()
                    .ForMember(m => m.RouteId, opt => opt.MapFrom(vm => vm.Route == null ? 0 : vm.Route.Id))
                    .ForMember(m => m.PlaneId, opt => opt.MapFrom(vm => vm.Plane == null ? 0 : vm.Plane.Id));
                cfg.CreateMap<DocumentViewModel, DocumentModel>()
                    .ForMember(m => m.Markers, opt => opt.MapFrom(vm => vm.Markers.Markers))
                    .ForMember(m => m.Routes, opt => opt.MapFrom(vm => vm.Routes.Routes))
                    .ForMember(m => m.Planes, opt => opt.MapFrom(vm => vm.Planes.Planes))
                    .ForMember(m => m.FlightPlans, opt => opt.MapFrom(vm => vm.FlightPlans.FlightPlans))
                    ;
                #endregion

                #region m -> vm

                cfg.CreateMap<CoordinateModel, CoordinateBaseViewModel>()
                    .ConstructUsing(m => new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService));
                cfg.CreateMap<FullCoordinateInformationModel, CoordinateBaseViewModel>()
                    .ConstructUsing(m => new CoordinateViewModel(_settingsService, _elevationService, _reverseGeocodeService, _airspaceInformationService))
                    .ForMember(vm => vm.Latitude, opt => opt.MapFrom(m => m.Coordinate.Latitude))
                    .ForMember(vm => vm.Longitude, opt => opt.MapFrom(m => m.Coordinate.Longitude))
                    ;
                cfg.CreateMap<MarkerModel, MarkerBaseViewModel>()
                    .ConstructUsing(m => new MarkerViewModel(_settingsService, _reverseGeocodeService, _elevationService, _airspaceInformationService))
                    .ForPath(vm => vm.Coordinate.Latitude, opt => opt.MapFrom(m => m.FullCoordinateInformationModel.Coordinate.Latitude))
                    .ForPath(vm => vm.Coordinate.Longitude, opt => opt.MapFrom(m => m.FullCoordinateInformationModel.Coordinate.Longitude))
                    .ForPath(vm => vm.Coordinate.DisplayName, opt => opt.MapFrom(m => m.FullCoordinateInformationModel.DisplayName))
                    .ForPath(vm => vm.Coordinate.Elevation, opt => opt.MapFrom(m => m.FullCoordinateInformationModel.Elevation))
                    .ForPath(vm => vm.Coordinate.City, opt => opt.MapFrom(m => m.FullCoordinateInformationModel.City))
                    ;
                cfg.CreateMap<RouteModel, RouteBaseViewModel>()
                    .ConstructUsing(m => new RouteViewModel())
                    .ForMember(vm => vm.Coordinates, opt => opt.MapFrom(m => m.Coordinates));
                cfg.CreateMap<PlaneModel, PlaneBaseViewModel>()
                    .ConstructUsing(m => new PlaneViewModel(_unitOfMeasureService, _settingsService));
                cfg.CreateMap<FlightPlanModel, FlightPlanBaseViewModel>()
                    .ConstructUsing(m => new FlightPlanViewModel());
                cfg.CreateMap<DocumentModel, DocumentViewModel>()
                    .ForPath(vm => vm.Markers.Markers, opt => opt.MapFrom(m => m.Markers))
                    .ForPath(vm => vm.Routes.Routes, opt => opt.MapFrom(m => m.Routes))
                    .ForPath(vm => vm.Planes.Planes, opt => opt.MapFrom(m => m.Planes))
                    .ForPath(vm => vm.FlightPlans.FlightPlans, opt => opt.MapFrom(m => m.FlightPlans))
                    .AfterMap((m, vm, rc) =>
                    {
                        foreach (var flightPlanVm in vm.FlightPlans.FlightPlans)
                        {
                            var flightPlanM = m.FlightPlans.SingleOrDefault(f => f.Id == flightPlanVm.Id);
                            if (flightPlanM != null)
                            {
                                flightPlanVm.Route = vm.Routes.Routes.SingleOrDefault(r => r.Id == flightPlanM.RouteId);
                                flightPlanVm.Plane = vm.Planes.Planes.SingleOrDefault(r => r.Id == flightPlanM.PlaneId);
                            }
                        }
                    })
                    ;
                #endregion
            });
            var mapper = mapperConfiguration.CreateMapper();

            return mapper;
        }
    }
}
