using AdsManagement.App.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.DTOs.User
{
    public class ResponseUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ResponseRoleDto Role {get; set;}
    }
}
