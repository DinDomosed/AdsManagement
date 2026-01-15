using AdsManagement.App.Interfaces.Events;

namespace AdsManagement.App.Services.Events
{
    public class CommentEventsDispatcher : ICommentEventsDispatcher
    {
        private readonly IEnumerable<ICommentEstimationChangedHandler> _handlers;
        public CommentEventsDispatcher(IEnumerable<ICommentEstimationChangedHandler> handlers)
        {
            _handlers = handlers;
        }
        public async Task CommentEstimationChangedAsync(Guid advertismentId, CancellationToken token)
        {
            await Task.WhenAll(_handlers.Select(h => h.HandleAsync(advertismentId, token)));
        }
    }
}
