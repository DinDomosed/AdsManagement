using AdsManagement.App.Exceptions;
using AdsManagement.App.Exceptions.NotFound;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;

namespace AdsManagement.App.Services
{
    public class AccessValidationsService : IAccessValidationsService
    {
        private readonly IAdvertisementStorage _adStorage;
        private readonly ICommentStorage _commentStorage;

        public AccessValidationsService(IAdvertisementStorage adStorage, ICommentStorage commentStorage)
        {
            _adStorage = adStorage;
            _commentStorage = commentStorage;
        }

        public async Task EnsureAdOwnerAsync(Guid adId, Guid requestUserId, CancellationToken token = default)
        {
            var adDb = await _adStorage.GetAsync(adId, token) ?? throw new AdvertisementNotFoundException(adId);
            EnsureOwner(adDb.UserId, requestUserId, adId);
        }
        public async Task EnsureCommentOwnerAsync(Guid commentId, Guid requestUserId, CancellationToken token = default)
        {
            var commentDb = await _commentStorage.GetAsync(commentId, token) ?? throw new CommentNotFoundException(commentId);
            EnsureOwner(commentDb.UserId, requestUserId, commentId);
        }
        public async Task EnsureUserOwnerAsync(Guid userId, Guid requestUserId, CancellationToken token = default)
        {
            EnsureOwner(userId, requestUserId, userId);
        }
        private static void EnsureOwner(Guid ownerId, Guid requestUserId, Guid resourceId)
        {
            if (ownerId != requestUserId)
                throw new AccessDeniedException(resourceId, requestUserId);
        }
    }
}

