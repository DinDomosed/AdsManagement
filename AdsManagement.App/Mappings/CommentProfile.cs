using AdsManagement.App.DTOs.Comment;
using AdsManagement.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CreateCommentDto, Comment>();

            CreateMap<Comment, ResponseCommentDto>(); 

            CreateMap<UpdateCommentDto, Comment>()
                .ForMember(dest => dest.AdvertisementId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());
        }
    }
}
