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
                    return context.Mapper.Map<List<ResponseAdImageDto>>(src.Images);
                }));

            CreateMap<CreateAdvertisementDto, Advertisement>()
                .ForMember(c => c.Number, opt => opt.Ignore());

            CreateMap<UpdateAdvertisementDto, Advertisement>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
