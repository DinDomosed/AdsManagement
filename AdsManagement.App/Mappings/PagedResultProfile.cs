using AdsManagement.App.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Mappings
{
    public class PagedResultProfile :Profile
    {
        public PagedResultProfile()
        {
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
