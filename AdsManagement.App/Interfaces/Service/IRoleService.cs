using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IRoleService
    {
        public Task<Guid> AddRoleAsync(CreateRoleDto roleDto, CancellationToken token = default);
        public Task<ResponseRoleDto> GetRoleAsync(Guid id, CancellationToken token = default);
        public Task<PagedResult<ResponseRoleDto>> GetAllRolesAsync(int page = 1, int pageSize = 10, CancellationToken token = default);
        public Task DeleteRoleAsync(Guid id, CancellationToken token = default);
        public Task UpdateRoleAsync(UpdateRoleDto roleDto, CancellationToken token = default);
    }
}
