namespace AdsManagement.App.DTOs.Advertisement
{
    public class AdFilterDto
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public decimal? Rating { get; set; }
        public int? Number { get; set; }

        public DateTime? CreatedDateFrom { get; set; }
        public DateTime? CreatedDateTo { get; set; }

        public DateTime? ExpiresDateFrom { get; set; }
        public DateTime? ExpiresDateTo { get; set; }

        public bool? IsExpired { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
