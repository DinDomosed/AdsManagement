using AdsManagement.App.Interfaces.Events;
using AdsManagement.App.Interfaces.Service;

namespace AdsManagement.App.Handlers
{
    public class RecalculateRatingHandler : ICommentEstimationChangedHandler
    {
        private readonly IAdvertisementService _service;
        public RecalculateRatingHandler(IAdvertisementService service)
        {
            _service = service;
        }
        public Task HandleAsync(Guid advertisementID, CancellationToken token)
        {
            return _service.RecalculateRatingAsync(advertisementID, token);
        }
    }
}
