using AdsManagement.App.Common;
using AdsManagement.App.DTOs.User;
using AdsManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces
{
    public interface IUserStorage: IStorage<User>
    {
        public Task<PagedResult<User>> GetFilterUserAsync(UserFilterDto userFilterDto);
    }
}
