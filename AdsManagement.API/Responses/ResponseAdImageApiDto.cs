namespace AdsManagement.API.Responses
{
    public class ResponseAdImageApiDto
    {
        public Guid Id { get; set; }
        public Guid AdvertisementId { get; set; }
        public string ImageURL { get; set; }
        public string SmallImageURL { get; set; }
    }
}
