using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.DTOs.User
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public Guid RoleId { get; set; }
    }
}
