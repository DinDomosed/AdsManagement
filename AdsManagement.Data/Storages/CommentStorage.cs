using AdsManagement.App.Common;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;

namespace AdsManagement.Data.Storages
{
    public class CommentStorage : ICommentStorage
    {
        private readonly AdsDbContext _context;
        public CommentStorage(AdsDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> AddAsync(Comment comment, CancellationToken token = default)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment), "The comment may not be null");

            if (!await UserExists(comment.UserId, token))
                throw new UserNotFoundException(comment.UserId);

            if (!await AdExists(comment.AdvertisementId, token))
                throw new AdvertisementNotFoundException(comment.AdvertisementId);

            if (await _context.Comments.AnyAsync(c => c.UserId == comment.UserId && c.AdvertisementId == comment.AdvertisementId))
                throw new CommentAlreadyExistsException("The comment already exists.");


            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(token);
            return comment.Id;
        }
        public async Task DeleteAsync(Guid commentId, CancellationToken token = default)
        {
            if (commentId == Guid.Empty)
                throw new ArgumentException(nameof(commentId), "The comment ID cannot be empty");

            var dbComment = await _context.Comments.FindAsync(commentId, token) ?? throw new CommentNotFoundException(commentId);

            _context.Comments.Remove(dbComment);
            await _context.SaveChangesAsync(token);
        }
        public async Task UpdateAsync(Comment comment, CancellationToken token = default)
        {
            if (comment == null)
                throw new ArgumentNullException(nameof(comment), "The comment may not be null");

            var dbComment = await _context.Comments.FindAsync(comment.Id, token) ?? throw new CommentNotFoundException(comment.Id);

            _context.Entry(dbComment).CurrentValues.SetValues(comment);
            await _context.SaveChangesAsync(token);
        }

        public async Task<PagedResult<Comment>> GetByAdvertisementAsync(Guid advertisementId, int page, int pageSize, CancellationToken token = default)
        {
            if (advertisementId == Guid.Empty)
                throw new ArgumentException(nameof(advertisementId), "The Advertisement ID cannot be empty");
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Comments.AsNoTracking().AsQueryable();
            query = query.Where(c => c.AdvertisementId == advertisementId);

            var totalCount = await query.CountAsync(token);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);

            return new PagedResult<Comment>()
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private async Task<bool> UserExists(Guid userId, CancellationToken token)
        {
            if (userId == Guid.Empty)
                return false;
            return await _context.Users.AnyAsync(c => c.Id == userId, token);
        }
        private async Task<bool> AdExists(Guid adId, CancellationToken token)
        {
            if (adId == Guid.Empty)
                return false;
            return await _context.Advertisements.AnyAsync(c => c.Id == adId, token);
        }
    }
}
