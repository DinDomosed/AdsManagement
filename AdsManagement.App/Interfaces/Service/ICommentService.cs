using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Comment;
using AdsManagement.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces.Service
{
    public interface ICommentService
    {
        public Task<ResponseCommentDto> GetCommentAsync(Guid id, CancellationToken token = default);
        public Task<Guid> AddCommentAsync(CreateCommentDto commentDto, CancellationToken token = default);
        public Task DeleteCommentAsync(Guid id, Guid requestUserId, CancellationToken token = default);
        public Task UpdateCommentAsync(UpdateCommentDto commentDto, Guid requestUserId, CancellationToken token = default);
        public Task<PagedResult<ResponseCommentDto>> GetByAdvertisementAsync(Guid advertisementId, int page = 1 , int pageSize = 10, CancellationToken token = default);
    }
}
