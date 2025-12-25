using AdsManagement.App.DTOs.AdImage;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.Domain.Models;
using AutoMapper;

namespace AdsManagement.App.Mappings
{
    public class AdvertisementProfile : Profile
    {
        public AdvertisementProfile()
        {
            CreateMap<Advertisement, ResponceAdvertisementDto>()
                .ForMember(d => d.Images, opt => opt.MapFrom((src, dest, srcMember, context) =>
                {
                    return context.Mapper.Map<ResponseAdImageDto>(src.Images);
                }));
        }
    }
}
