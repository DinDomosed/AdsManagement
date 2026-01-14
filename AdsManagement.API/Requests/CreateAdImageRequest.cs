using System.ComponentModel.DataAnnotations;

namespace AdsManagement.API.Requests
{
    public class CreateAdImageRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
