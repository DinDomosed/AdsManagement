using AdsManagement.App.DTOs.AdImage;

namespace AdsManagement.App.DTOs.Advertisement
{
    public class ResponceAdvertisementDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<ResponseAdImageDto> Images { get; set;}
    }
}
