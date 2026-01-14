using AdsManagement.API.Adapters;
using AdsManagement.API.Requests;
using AdsManagement.API.Responses;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace AdsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdImageController : ControllerBase 
    {
        private readonly IAdImageService _service;
        public AdImageController(IAdImageService service)
        {
            _service = service;
        }
        [HttpGet("byad/{adId}")]
        public async Task<IActionResult> GetByAd([FromRoute] Guid adId, CancellationToken token = default)
        {
            var images = await _service.GetByAdIdAsync(adId, token);

            var response = images.Select(c => new ResponseAdImageApiDto
            {
                AdvertisementId = c.AdvertisementId,
                Id = c.Id,
                ImageURL = Url.Action(nameof(GetAdImage), new { id = c.Id }),
                SmallImageURL = Url.Action(nameof(GetAdImageSmall), new { id = c.Id })
            })
                .ToList();

            return Ok(response);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdImage([FromRoute] Guid id, CancellationToken token = default)
        {
            var image = await _service.GetImageAsync(id, token);
            var path = image.OriginalImagePath;

            if (!System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetImageContentType(path);

            var stream = System.IO.File.OpenRead(path);
            return File(stream, contentType);
        }
        [HttpGet("{id}/small")]
        public async Task<IActionResult> GetAdImageSmall([FromRoute] Guid id, CancellationToken token = default)
        {
            var image = await _service.GetImageAsync(id, token);
            var path = image.SmallImagePath;

            if ( !System.IO.File.Exists(path))
                return NotFound();

            var contentType = GetImageContentType(path);

            var stream = System.IO.File.OpenRead(path);
            return File(stream, contentType);
        }
        [Consumes("multipart/form-data")]
        [HttpPost("{adId}")]
        public async Task<IActionResult> CreateAdImage([FromRoute] Guid adId, [FromHeader(Name = "X-User-ID")] Guid requestUserId, [FromForm] CreateAdImageRequest file, CancellationToken token = default)
        {
            if (file == null || file.File.Length == 0)
                return BadRequest("The image being added cannot be empty");
            var fileApapter = new FormFileDataAdapter(file.File);

            var imageId = await _service.AddAdImageAsync(adId, fileApapter, requestUserId, token);

            var dbImage = await _service.GetImageAsync(imageId, token);
            var response = new ResponseAdImageApiDto()
            {
                AdvertisementId = dbImage.AdvertisementId,
                Id = dbImage.Id,
                ImageURL = Url.Action(nameof(GetAdImage), new { id = dbImage.Id }),
                SmallImageURL = Url.Action(nameof(GetAdImageSmall), new { id = dbImage.Id })
            };

            return CreatedAtAction(nameof(GetAdImage), new { id = dbImage.Id }, response);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdImage([FromRoute] Guid id, [FromHeader(Name = "X-User-ID")] Guid requestUserId, CancellationToken token = default)
        {
            await _service.DeleteAdImageAsync(id, requestUserId, token);
            return NoContent();
        }
        private string GetImageContentType(string path)
        {
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return ext switch
            {
                ".jpeg" => "image/jpeg",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }
    }
}
