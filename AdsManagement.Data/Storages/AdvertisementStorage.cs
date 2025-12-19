using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using static System.Net.Mime.MediaTypeNames;

namespace AdsManagement.Data.Storages
{
    public class AdvertisementStorage : IAdvertisementStorage
    {
        private readonly int _limitationAds;
        private readonly AdsDbContext _context;
        private readonly IDateTimeProvider _time;
        public AdvertisementStorage(AdsDbContext context, IDateTimeProvider time, int limitationAds)
        {
            _context = context;
            _time = time;
            _limitationAds = limitationAds;
        }
        public async Task<Advertisement> GetAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The advertisement ID cannot be empty");

            var dbAdv = await _context.Advertisements
                .AsNoTracking()
                .Include(c => c.Images)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id, token) ?? throw new AdvertisementNotFoundException(id);

            return dbAdv;
        }

        public async Task<Guid> AddAsync(Advertisement advertisement, CancellationToken token = default)
        {
            if (advertisement == null)
                throw new ArgumentNullException(nameof(advertisement), "The advertisement cannot be null");

            var dbUser = await GetUserAsync(advertisement.UserId, token);

            if (await GetUserAdsCountActive(dbUser.Id, token) >= _limitationAds)
                throw new ExceedingTheAdLimitException("The limit of active ads has been reached");

            _context.Advertisements.Add(advertisement);
            await _context.SaveChangesAsync(token);
            return advertisement.Id;
        }

        public async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                throw new ArgumentException(nameof(id), "The ID cannot be empty");

            var dbAdv = await _context.Advertisements.FindAsync(id, token);

            if (dbAdv == null)
                throw new AdvertisementNotFoundException(id);

            _context.Advertisements.Remove(dbAdv);
            await _context.SaveChangesAsync(token);
        }
        public async Task UpdateAsync(Advertisement adv, CancellationToken token = default)
        {
            if (adv == null)
                throw new ArgumentNullException(nameof(adv), "The advertisement cannot be null");

            var dbAdv = await _context.Advertisements.FindAsync(adv.Id, token) ?? throw new AdvertisementNotFoundException(adv.Id);

            _context.Entry(dbAdv).CurrentValues.SetValues(adv);
            await _context.SaveChangesAsync(token);
        }

        public async Task<PagedResult<Advertisement>> GetFilterAdsAsync(AdFilterDto filter, CancellationToken token = default)
        {
            var query = _context.Advertisements.AsNoTracking()
                .Include(c => c.Images)
                .AsQueryable();

            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 10;

            query = ApplyFilters(query, filter);
            query = ApplySorting(query, filter.SortBy, filter.SortDesc);

            int totalCount = await query.CountAsync(token);
            var items = await query
                .AsNoTracking()
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(token);

            return new PagedResult<Advertisement>()
            {
                TotalCount = totalCount,
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }
        public async Task<int> GetUserAdsCountAll(Guid userId, CancellationToken token = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(nameof(userId), "The ID cannot be empty");

            var query = _context.Advertisements.AsNoTracking().AsQueryable();

            query = query.Where(c => c.UserId == userId);
            return await query.CountAsync(token);
        }

        public async Task<int> GetUserAdsCountActive(Guid userId, CancellationToken token = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException(nameof(userId), "The ID cannot be empty");

            var query = _context.Advertisements.AsNoTracking().AsQueryable();

            query = query.Where(c => c.UserId == userId && c.ExpiresAt >= _time.UtcNow);
            return await query.CountAsync(token);

        }
        public async Task<PagedResult<Advertisement>> GetUserAdsAsync(Guid userId, UserAdvertisementFilterDto filter,
            CancellationToken token = default)
        {
            if (userId == Guid.Empty)
                throw new ArgumentNullException(nameof(userId), "The user ID cannot be empty");

            if (!await _context.Users.AnyAsync(c => c.Id == userId, token))
                throw new UserNotFoundException(userId);

            if (filter.Page <= 0) filter.Page = 1;
            if (filter.PageSize <= 0) filter.PageSize = 10;

            var query = _context.Advertisements
                .AsNoTracking()
                .Include(c => c.Images)
                .Where(c => c.UserId == userId)
                .AsQueryable();

            switch (filter.IsExpired)
            {
                case true:
                    query = query.Where(c => c.ExpiresAt < _time.UtcNow);
                    break;

                case false:
                    query = query.Where(c => c.ExpiresAt >= _time.UtcNow);
                    break;
            }

            int totalCount = await query.CountAsync(token);

            var items = await query
                .OrderByDescending(c => c.Number)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(token);

            return new PagedResult<Advertisement>()
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        private async Task<User?> GetUserAsync(Guid id, CancellationToken token = default)
        {
            if (id == Guid.Empty)
                return null;

            return await _context.Users.FindAsync(id, token) ?? throw new UserNotFoundException(id);
        }
        private IQueryable<Advertisement> ApplyFilters(IQueryable<Advertisement> query, AdFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                var title = filter.Title.Trim();

                if (_context.Database.ProviderName?.Contains("InMemory") == true) //for tests
                    query = query.Where(c => c.Title.StartsWith(title, StringComparison.OrdinalIgnoreCase));

                else
                    query = query.Where(c => EF.Functions.ILike(c.Title, $"{title}%"));
            }

            if (!string.IsNullOrWhiteSpace(filter.Text))
                query = query.Where(c => EF.Functions.ToTsVector("russian", c.Text)
                .Matches(filter.Text));

            if (filter.Rating.HasValue)
                query = query.Where(c => c.Rating == filter.Rating);

            if (filter.Number.HasValue)
                query = query.Where(c => c.Number == filter.Number);

            if (filter.CreatedDateFrom.HasValue)
                query = query.Where(c => c.CreatedAt >= filter.CreatedDateFrom);

            if (filter.CreatedDateTo.HasValue)
                query = query.Where(c => c.CreatedAt <= filter.CreatedDateTo);

            if (filter.ExpiresDateFrom.HasValue)
                query = query.Where(c => c.ExpiresAt >= filter.ExpiresDateFrom);

            if (filter.ExpiresDateTo.HasValue)
                query = query.Where(c => c.ExpiresAt <= filter.ExpiresDateTo);

            if (filter.IsExpired == true)
                query = query.Where(c => c.ExpiresAt < _time.UtcNow);
            else if (filter.IsExpired == false)
                query = query.Where(c => c.ExpiresAt >= _time.UtcNow);

            return query;
        }
        private IQueryable<Advertisement> ApplySorting(IQueryable<Advertisement> query, string? sortBy, bool? sortDesc)
        {
            switch (sortBy?.ToLower())
            {
                case "title":
                    query = sortDesc == true
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title);
                    break;

                case "rating":
                    query = sortDesc == true
                        ? query.OrderByDescending(c => c.Rating)
                        : query.OrderBy(c => c.Rating);
                    break;

                case "number":
                    query = sortDesc == true
                        ? query.OrderByDescending(c => c.Number)
                        : query.OrderBy(c => c.Number);
                    break;

                case "creationDate":
                    query = sortDesc == true
                        ? query.OrderByDescending(c => c.CreatedAt)
                        : query.OrderBy(c => c.CreatedAt);
                    break;

                default:
                    query = sortDesc == true
                        ? query.OrderByDescending(c => c.Title)
                        : query.OrderBy(c => c.Title);
                    break;
            }

            return query;
        }
    }
}

