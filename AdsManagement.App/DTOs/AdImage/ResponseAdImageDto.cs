namespace AdsManagement.App.DTOs.AdImage
{
    public class ResponseAdImageDto
    {
        public Guid Id { get; set; }
        public Guid AdvertisementId { get;  set; }
        public string OriginalImagePath { get;  set; }
        public string SmallImagePath { get;  set; }
    }
}
