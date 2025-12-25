using AdsManagement.App.DTOs.AdImage;
using AdsManagement.Domain.Models;
using AutoMapper;

namespace AdsManagement.App.Mappings
{
    public class AdImageProfile : Profile
    {
        public AdImageProfile()
        {
            CreateMap<AdvertisementImage, ResponseAdImageDto>();
        }
    }
}
