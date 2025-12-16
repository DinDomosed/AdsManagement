using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.DTOs.User;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AdsManagement.Data.Storages
{
    public class UserStorage : IUserStorage
    {
        private readonly AdsDbContext _dbContext;

        public UserStorage(AdsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User?> GetAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                return null;
            return await _dbContext.Users.AsNoTracking()
                .Include(c => c.Role)
                .FirstOrDefaultAsync(c => c.Id == id, token);
        }

        public async Task<bool> AddAsync(User user, CancellationToken token = default)
        {
            if (user == null)
                return false;

            var dbRole = await GetAndAttachRoleAsync(user.RoleId, token);

            user.UpdateRole(dbRole);

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(token);

            return true;
        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                return false;

            var dbUser = await _dbContext.Users.FindAsync(id, token);

            if (dbUser == null)
                throw new UserNotFoundException($"User {id} not found");

            _dbContext.Users.Remove(dbUser);
            await _dbContext.SaveChangesAsync(token);

            return true;
        }
        public async Task<bool> UpdateAsync(User user, CancellationToken token = default)
        {
            if (user == null)
                return false;

            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(c => c.Id == user.Id);

            if (dbUser == null)
                throw new UserNotFoundException($"User {user.Id} not found");

            if (dbUser.RoleId != user.RoleId)
            {
                var dbRole = await GetAndAttachRoleAsync(user.RoleId, token);

                dbUser.UpdateRole(dbRole);
            }

            _dbContext.Entry(dbUser).CurrentValues.SetValues(user);
            await _dbContext.SaveChangesAsync(token);

            return true;
        }

        public async Task<PagedResult<User>> GetFilterUserAsync(UserFilterDto filterDto, CancellationToken token = default)
        {
            var query = _dbContext.Users
                .Include(c => c.Role)
                .AsNoTracking()
                .AsQueryable();

            if (filterDto.Page <= 0) filterDto.Page = 1;
            if (filterDto.PageSize <= 0) filterDto.PageSize = 10;

            if (!string.IsNullOrWhiteSpace(filterDto.Name))
            {
                var name = filterDto.Name.Trim();
                if (_dbContext.Database.ProviderName?.Contains("InMemory") == true)
                        query = query.Where(c => c.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase));
                else
                    query = query.Where(c => EF.Functions.Like(c.Name, $"{name}%"));
            }

            if (filterDto.RoleId.HasValue)
                query = query.Where(c => c.RoleId == filterDto.RoleId);

            int totalCount = await query.CountAsync(token);

            query = ApplySorting(query, filterDto.SortBy, filterDto.SortDesc);

            List<User> items = await query
                .Skip((filterDto.Page - 1) * filterDto.PageSize)
                .Take(filterDto.PageSize)
                .AsNoTracking()
                .ToListAsync(token);

            return new PagedResult<User>()
            {
                Items = items,
                Page = filterDto.Page,
                PageSize = filterDto.PageSize,
                TotalCount = totalCount
            };
        }

        private async Task<Role> GetAndAttachRoleAsync(Guid roleId, CancellationToken token = default)
        {
            if (roleId == Guid.Empty)
                throw new ArgumentNullException(nameof(roleId), "Role ID cannot be empty");

            var dbRole = _dbContext.Roles.Local.FirstOrDefault(c => c.Id == roleId)
               ?? await _dbContext.Roles.FirstOrDefaultAsync(c => c.Id == roleId, token);

            if (dbRole == null)
                throw new RoleNotFoundException($"Role '{roleId}' not found");
            else
            {
                if (_dbContext.Entry(dbRole).State == EntityState.Detached)
                    _dbContext.Roles.Attach(dbRole);
            }
            return dbRole;

        }

        private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool? sortDesc)
        {
            switch (sortBy?.ToLower())
            {
                case "name":
                    query = sortDesc == true
                    ? query = query.OrderByDescending(c => c.Name)
                    : query = query.OrderBy(c => c.Name);
                    break;

                case "role":

                    query = sortDesc == true
                    ? query = query.OrderByDescending(c => c.Role.Name)
                    : query = query.OrderBy(c => c.Role.Name);
                    break;

                default:
                    query = sortDesc == true
                    ? query = query.OrderByDescending(c => c.Name)
                    : query = query.OrderBy(c => c.Name);
                    break;
            }
            return query;
        }
    }
}
