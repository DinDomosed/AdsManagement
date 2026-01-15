namespace AdsManagement.API.Responses
{
    public class ResponseAdApiDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public decimal Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public List<ResponseAdImageApiDto>? Images { get; set; }
    }
}
