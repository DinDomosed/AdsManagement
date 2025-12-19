using AdsManagement.App.Common;
using AdsManagement.App.DTOs.User;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Services
{
    public class UserService : IUserService
    {
        private readonly IUserStorage _storage;
        private readonly IMapper _mapper;

        public UserService(IUserStorage storage, IMapper mapper)
        {
            _storage = storage;
            _mapper = mapper;
        }
        public async Task<ResponseUserDto> GetUserAsync(Guid id, CancellationToken token = default)
        {
            var dbUser = await _storage.GetAsync(id, token);
            return _mapper.Map<ResponseUserDto>(dbUser);
        }
        public async Task<Guid> AddUserAsync(CreateUserDto userDto, CancellationToken token = default)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "The user cannot be null");

            var user = _mapper.Map<User>(userDto);
            return await _storage.AddAsync(user, token);
        }
        public async Task DeleteUserAsync(Guid id, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            if (id != requestUserId)
                throw new AccessDeniedException(id, requestUserId);

            await _storage.DeleteAsync(id, token);
        }
        public async Task UpdateUserAsync(UpdateUserDto userDto, Guid requestUserId, CancellationToken token = default)
        {
            if (userDto == null)
                throw new ArgumentNullException(nameof(userDto), "The user cannot be null");

            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            if (userDto.Id != requestUserId)
                throw new AccessDeniedException(userDto.Id, requestUserId);

            var user = _mapper.Map<User>(userDto);
            await _storage.UpdateAsync(user, token);
        }

        public async Task<PagedResult<ResponseUserDto>> GetFilterUserAsync(UserFilterDto filter, CancellationToken token = default)
        {
            var pages = await _storage.GetFilterUserAsync(filter, token);
            return _mapper.Map<PagedResult<ResponseUserDto>>(pages);
        }
    }
}
