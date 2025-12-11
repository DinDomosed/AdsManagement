using AdsManagement.App.Common;
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
                .Include(c => c.Advertisements)
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
                return false;

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
                return false;

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

            if (!string.IsNullOrWhiteSpace(filterDto.Name))
                query = query.Where(c => c.Name.Contains(filterDto.Name));

            if (filterDto.RoleId.HasValue)
                query = query.Where(c => c.RoleId == filterDto.RoleId);

            int totalCount = await query.CountAsync(token);

            switch (filterDto.SortBy)
            {
                case "Name":
                    {
                        if (filterDto.SortDesc == true)
                        {
                            query = query.OrderByDescending(c => c.Name);

                        }
                        else
                        {
                            query = query.OrderBy(c => c.Name);
                        }
                        break;
                    }
                case "Role":
                    {
                        if (filterDto.SortDesc == true)
                        {
                            query = query.OrderByDescending(c => c.Role.Name);
                        }
                        else
                        {
                            query = query.OrderBy(c => c.Role.Name);
                        }
                        break;
                    }
            }

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
    }
}
