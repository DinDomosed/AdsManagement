namespace AdsManagement.App.DTOs.Comment
{
    public class ResponseCommentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Text { get; set; }
        public int Estimation { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
