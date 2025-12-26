using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Comment;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using AutoMapper;

namespace AdsManagement.App.Services
{
    public class CommentService : ICommentService
    {
        public event Func<Guid, Task> CommentEstinationChanged;
        private readonly ICommentStorage _storage;
        private readonly IAccessValidationsService _accessValidations;
        private readonly IMapper _mapper;

        public CommentService(ICommentStorage storage, IMapper mapper, IAccessValidationsService accessValidations)
        {
            _storage = storage;
            _mapper = mapper;
            _accessValidations = accessValidations;
        }
        public async Task<Guid> AddCommentAsync(CreateCommentDto commentDto, CancellationToken token = default)
        {
            if (commentDto == null)
                throw new ArgumentNullException(nameof(commentDto), "The comment cannot be null");

            var comment = _mapper.Map<Comment>(commentDto);
            var dbIDComment = await _storage.AddAsync(comment, token);

            OnCommentEstinationChanged(commentDto.AdvertisementId);

            return dbIDComment;
        }
        public async Task DeleteCommentAsync(Guid id, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            var dbComment = await _storage.GetAsync(id, token);

            await _accessValidations.EnsureCommentOwnerAsync(id, requestUserId, token);

            await _storage.DeleteAsync(id, token);

            OnCommentEstinationChanged(dbComment.AdvertisementId);
        }
        public async Task UpdateCommentAsync(UpdateCommentDto commentDto, Guid requestUserId, CancellationToken token = default)
        {
            if (commentDto == null)
                throw new ArgumentNullException(nameof(commentDto), "The comment cannot be null");
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            var dbComment = await _storage.GetAsync(commentDto.Id, token);

            await _accessValidations.EnsureCommentOwnerAsync(dbComment.Id, requestUserId, token);

            var comment = _mapper.Map<Comment>(commentDto);

            await _storage.UpdateAsync(comment, token);

            if (dbComment.Estimation != commentDto.Estimation)
                OnCommentEstinationChanged(dbComment.AdvertisementId);
        }
        public async Task<PagedResult<ResponceCommentDto>> GetByAdvertisementAsync(Guid advertisementId, int page = 1, int pageSize = 10,
            CancellationToken token = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 4) pageSize = 5;

            var pages = await _storage.GetByAdvertisementAsync(advertisementId, page, pageSize, token);
            return _mapper.Map<PagedResult<ResponceCommentDto>>(pages);
        }
        protected virtual void OnCommentEstinationChanged(Guid advertisementId)
        {
            CommentEstinationChanged?.Invoke(advertisementId);
        }
    }
}
