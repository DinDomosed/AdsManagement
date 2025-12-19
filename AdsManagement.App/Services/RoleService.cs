using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Role;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain;
using AdsManagement.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleStorage _storage;
        private readonly IMapper _mapper;
        public RoleService(IRoleStorage storage, IMapper mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }
        public async Task<Guid> AddRoleAsync(CreateRoleDto roleDto, CancellationToken token = default)
        {
            if (roleDto == null)
                throw new ArgumentNullException(nameof(roleDto), "The role cannot be null");

            var role = _mapper.Map<Role>(roleDto);

            return await _storage.AddAsync(role, token);
        }
        public async Task<ResponseRoleDto> GetRoleAsync(Guid id, CancellationToken token = default)
        {
            var dbRole = await _storage.GetAsync(id, token);
            return _mapper.Map<ResponseRoleDto>(dbRole);
        }
        public async Task<PagedResult<ResponseRoleDto>> GetAllRolesAsync(int page = 1, int pageSize = 10, CancellationToken token = default)
        {
            var dbRoles = await _storage.GetAllAsync(page, pageSize, token);
            return _mapper.Map<PagedResult<ResponseRoleDto>>(dbRoles);
        }
        public async Task DeleteRoleAsync(Guid id, CancellationToken token = default)
        {
            await _storage.DeleteAsync(id, token);
        }
        public async Task UpdateRoleAsync(UpdateRoleDto roleDto, CancellationToken token = default)
        {
            if(roleDto == null)
                throw new ArgumentNullException(nameof(roleDto), "The role cannot be null");

            var role = _mapper.Map<Role>(roleDto);
            await _storage.UpdateAsync(role, token);
        }
    }
}
