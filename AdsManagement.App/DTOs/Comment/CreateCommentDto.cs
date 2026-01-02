namespace AdsManagement.App.DTOs.Comment
{
    public class CreateCommentDto
    {
        public Guid UserId { get; set; }
        public Guid AdvertisementId { get; set; }
        public string Text { get; set; }
        public int Estimation { get; set; }
    }
}
