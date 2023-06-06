using AutoMapper;
using FileDocument.Models.Dtos;
using FileDocument.Models.Entities;

namespace FileDocumentManagementSystem
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AddressDto, Address>().ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<RegisterDto, User>().ForAllMembers(otp => otp.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<SystemConfigureDto, SystemConfigure>();
        }
    }
}
