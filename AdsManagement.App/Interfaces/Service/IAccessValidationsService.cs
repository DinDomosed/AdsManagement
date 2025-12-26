using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdsManagement.App.Interfaces.Service
{
    public interface IAccessValidationsService
    {
        public Task EnsureAdOwnerAsync(Guid adId, Guid requestUserId, CancellationToken token = default);
        public Task EnsureCommentOwnerAsync(Guid commentId, Guid requestUserId, CancellationToken token = default);
        public Task EnsureUserOwnerAsync(Guid userId, Guid requestUserId, CancellationToken token = default);

    }
}
