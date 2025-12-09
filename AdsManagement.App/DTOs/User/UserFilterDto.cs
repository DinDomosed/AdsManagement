using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.DTOs.User
{
    public class UserFilterDto
    {
        public string? Name { get; set; }
        public Guid? RoleId { get; set; }

        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
