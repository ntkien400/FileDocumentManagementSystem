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
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom(src => src.DepartureDate))
                .ForMember(dest => dest.DepartureDate, opt => opt.MapFrom<StringToDateResolver>())
                .ForMember(dest => dest.ArrivalDate, opt => opt.MapFrom(src => src.DepartureDate))
                .ForMember(dest => dest.ArrivalDate, opt => opt.MapFrom<StringToDateResolver>())
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom<StringToTimeSpanResolver>())
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom(src => src.ArrivalTime))
                .ForMember(dest => dest.ArrivalTime, opt => opt.MapFrom<StringToTimeSpanResolver>());
        }

        public class StringToDateResolver : IValueResolver<FlightDto, Flight, DateTime>
        {
            public DateTime Resolve( FlightDto source, Flight destination, DateTime destMember, ResolutionContext context)
            {
                DateTime date1, date2;
                var result1 = DateTime.TryParseExact(source.DepartureDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date1);
                var result2 = DateTime.TryParseExact(source.ArrivalDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date2);
                if (result1)
                {
                    destination.DepartureDate = date1;
                }

                if (result2)
                {
                    destination.ArrivalDate = date2;
                }

                return default(DateTime);
            }
        }

        public class StringToTimeSpanResolver : IValueResolver<FlightDto, Flight, TimeSpan>
        {
            public TimeSpan Resolve(FlightDto source, Flight destination, TimeSpan destMember, ResolutionContext context)
            {
                TimeSpan time1, time2;
                var result1 = TimeSpan.TryParseExact(source.DepartureTime, "hh\\:mm", CultureInfo.InvariantCulture, out time1);
                var result2 = TimeSpan.TryParseExact(source.ArrivalTime, "hh\\:mm", CultureInfo.InvariantCulture, out time2);
                if(result1)
                {
                    destination.DepartureTime = time1;
                }
                if (result2)
                {
                    destination.ArrivalTime = time2;
                }

                return default(TimeSpan);
            }
        }

    }
}
