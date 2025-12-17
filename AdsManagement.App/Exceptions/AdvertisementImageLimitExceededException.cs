namespace AdsManagement.App.Exceptions
{
    public class AdvertisementImageLimitExceededException : Exception
    {
        public int ImageLimit  { get; }
        public Guid AdvertisementId { get; }
        public AdvertisementImageLimitExceededException(Guid advertisementId, int imageLimit) : base($"Image limit ({imageLimit}) exceeded for advertisement {advertisementId}") 
        {
            ImageLimit  = imageLimit;
            AdvertisementId = advertisementId;
        }
    }
}
