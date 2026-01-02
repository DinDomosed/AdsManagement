namespace AdsManagement.App.DTOs.Comment
{
    public class UpdateCommentDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public int Estimation { get; set; }
    }
}
