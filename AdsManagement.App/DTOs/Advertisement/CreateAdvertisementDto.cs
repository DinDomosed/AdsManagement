namespace AdsManagement.App.DTOs.Advertisement
{
    public class CreateAdvertisementDto
    {
        public Guid UserId { get;  set; }
        public string Title { get;  set; }
        public string Text { get;  set; }
    }
}
