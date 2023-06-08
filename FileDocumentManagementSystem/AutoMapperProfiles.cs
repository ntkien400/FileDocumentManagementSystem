using AutoMapper;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;
using System.Globalization;

namespace FileDocumentManagementSystem
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddressDto, Address>();
            CreateMap<RegisterDto, User>();
            CreateMap<SystemConfigureDto, SystemConfigure>();
            CreateMap<AircraftDto, Aircraft>();
            CreateMap<FlightDto, Flight>()
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.DepartureDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.ArrivalDate, opt => opt.MapFrom(src => DateTime.ParseExact(src.ArrivalDate, "dd/MM/yyyy", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => TimeSpan.ParseExact(src.DepartureTime, "hh\\:mm", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => TimeSpan.ParseExact(src.ArrivalTime, "hh\\:mm", CultureInfo.InvariantCulture)));
            CreateMap<DocumentDto, Document>();
        }

    }
}
