using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces
{
    public interface IAdvertisementStorage : IStorage<Advertisement>
    {
        public Task<PagedResult<Advertisement>> GetFilterAdsAsync(AdFilterDto adFilterDto);
        public Task<int> GetUserAdsCountActive(Guid userId);
        public Task<int> GetUserAdsCountAll(Guid userId);
    }
}
