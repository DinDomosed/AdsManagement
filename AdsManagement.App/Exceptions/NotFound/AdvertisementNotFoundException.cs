using AdsManagement.Domain.Models;

namespace AdsManagement.App.Exceptions.NotFound
{
    public sealed class AdvertisementNotFoundException : EntityNotFoundException
    {
        public AdvertisementNotFoundException(Guid id) : base (nameof(Advertisement), id) { }
    }
}
