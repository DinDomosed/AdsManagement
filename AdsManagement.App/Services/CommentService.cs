using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Comment;
using AdsManagement.App.Exceptions;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.App.Interfaces.Events;
using AdsManagement.App.Interfaces.Storage;
using AdsManagement.Domain.Models;
using AutoMapper;
using System.Threading.Tasks;

namespace AdsManagement.App.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentStorage _storage;
        private readonly IAccessValidationsService _accessValidations;
        private readonly IMapper _mapper;
        private readonly ICommentEventsDispatcher _dispatcher;

        public CommentService(ICommentStorage storage, IMapper mapper, IAccessValidationsService accessValidations, ICommentEventsDispatcher dispatcher)
        {
            _storage = storage;
            _mapper = mapper;
            _accessValidations = accessValidations;
            _dispatcher = dispatcher;
        }
        public async Task<ResponseCommentDto> GetCommentAsync(Guid id, CancellationToken token = default)
        {
            var comment = await _storage.GetAsync(id, token);
            var response = _mapper.Map<ResponseCommentDto>(comment);
            return response;
        }
        public async Task<Guid> AddCommentAsync(CreateCommentDto commentDto, CancellationToken token = default)
        {
            if (commentDto == null)
                throw new ArgumentNullException(nameof(commentDto), "The comment cannot be null");

            var comment = _mapper.Map<Comment>(commentDto);
            var dbIDComment = await _storage.AddAsync(comment, token);

            await OnCommentEstimationChanged(commentDto.AdvertisementId);

            return dbIDComment;
        }
        public async Task DeleteCommentAsync(Guid id, Guid requestUserId, CancellationToken token = default)
        {
            if (requestUserId == Guid.Empty)
                throw new ArgumentException(nameof(requestUserId), "The ID of the user who sent the request cannot be empty");

            var dbComment = await _storage.GetAsync(id, token);

            await _accessValidations.EnsureCommentOwnerAsync(id, requestUserId, token);

            await _storage.DeleteAsync(id, token);

            await OnCommentEstimationChanged(dbComment.AdvertisementId);
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
                await OnCommentEstimationChanged(dbComment.AdvertisementId, token);
        }
        public async Task<PagedResult<ResponseCommentDto>> GetByAdvertisementAsync(Guid advertisementId, int page = 1, int pageSize = 10,
            CancellationToken token = default)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 4) pageSize = 5;

            var pages = await _storage.GetByAdvertisementAsync(advertisementId, page, pageSize, token);
            return _mapper.Map<PagedResult<ResponseCommentDto>>(pages);
        }
        protected virtual async Task OnCommentEstimationChanged(Guid advertisementId, CancellationToken token = default)
        {
            await _dispatcher.CommentEstimationChangedAsync(advertisementId, token);
        }
    }
}
