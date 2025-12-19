using AdsManagement.App.DTOs.Role;
using AdsManagement.App.DTOs.User;
using AdsManagement.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, ResponseUserDto>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom((src, dest, srcMember, context) =>
                {
                    return context.Mapper.Map<ResponseRoleDto>(src.Role);
                }))
                .ForAllMembers(options => options.Condition((src, dest, srcMember, destMember) =>
                {
                    if (srcMember == null)
                        return false;

                    if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                        return false;

                    if (srcMember != null && srcMember.Equals(destMember))
                        return false;

                    return true;
                }));

            CreateMap<CreateUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
                {
                    if (srcMember == null)
                        return false;

                    if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                        return false;

                    if (srcMember != null && srcMember == destMember)
                        return false;

                    return true;
                }));

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
                {
                    if (srcMember == null)
                        return false;

                    if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                        return false;

                    if (srcMember != null && srcMember == destMember)
                        return false;

                    return true;
                }));


        }
    }
}
