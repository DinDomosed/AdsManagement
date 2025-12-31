namespace AdsManagement.App.DTOs.Advertisement
{
    public class UserAdvertisementFilterDto
    {
        public bool? IsExpired { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
