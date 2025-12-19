using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Role;
using AdsManagement.Domain.Models;
using AutoMapper;

namespace AdsManagement.App.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<CreateRoleDto, Role>()
                .ConstructUsing(src => new Role(src.Name, null));

            CreateMap<Role, ResponseRoleDto>();

            CreateMap<UpdateRoleDto, Role>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember, destMember) =>
                {
                    if (srcMember == null)
                        return false;

                    if (srcMember is string s && string.IsNullOrWhiteSpace(s))
                        return false;

                    if (srcMember != null && srcMember.Equals(destMember))
                        return false;

                    return true;
                }));
        }
    }
}
