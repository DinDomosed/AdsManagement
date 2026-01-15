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
            CreateMap<Advertisement, ResponseAdvertisementDto>()
                .ForMember(d => d.Images, opt => opt.MapFrom((src, dest, srcMember, context) =>
                {
                    return context.Mapper.Map<List<ResponseAdImageDto>>(src.Images);
                }));

            CreateMap<CreateAdvertisementDto, Advertisement>()
                .ConstructUsing(c => new Advertisement(
                    c.UserId,
                    c.Title,
                    0,
                    c.Text,
                    null));

            CreateMap<UpdateAdvertisementDto, Advertisement>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
        }
    }
}
