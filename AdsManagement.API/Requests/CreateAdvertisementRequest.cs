using AdsManagement.App.DTOs.Advertisement;

namespace AdsManagement.API.Requests
{
    public class CreateAdvertisementRequest
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public IFormFile? FormFile { get; set; }
    }
}
