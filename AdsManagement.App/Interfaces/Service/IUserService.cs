using AdsManagement.App.Common;
using AdsManagement.App.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IUserService
    {
        public Task<ResponseUserDto> GetUserAsync(Guid id, CancellationToken token = default);
        public Task<Guid> AddUserAsync(CreateUserDto userDto, CancellationToken token = default);

        /// paramName = RequestUserId  => ID пользователя, выполняющего запрос (используется для проверки прав)
        public Task DeleteUserAsync(Guid id, Guid requestUserId, CancellationToken token = default);
        public Task UpdateUserAsync(UpdateUserDto userDto, Guid requestUserId, CancellationToken token = default);
        public Task<PagedResult<ResponseUserDto>> GetFilterUserAsync(UserFilterDto filterDto, CancellationToken token = default);

    }
}
