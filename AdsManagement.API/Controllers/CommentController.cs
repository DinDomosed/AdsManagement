using AdsManagement.App.DTOs.Comment;
using AdsManagement.App.Interfaces.Service;
using Microsoft.AspNetCore.Mvc;

namespace AdsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _service;
        public CommentController(ICommentService service)
        {
            _service = service;
        }
        [HttpGet("{adId}")]
        public async Task<IActionResult> GetByAdComment([FromRoute] Guid adId, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken token = default)
        {
            var response = await _service.GetByAdvertisementAsync(adId, page, pageSize, token);
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto commentDto, CancellationToken token = default)
        {
            var result = await _service.AddCommentAsync(commentDto, token);
            var response = await _service.GetCommentAsync(result, token);
            return CreatedAtAction(nameof(GetByAdComment), new { adId = commentDto.AdvertisementId }, response);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid id, [FromHeader(Name ="X-User-ID")] Guid requestUserId, CancellationToken token = default)
        {
            await _service.DeleteCommentAsync(id, requestUserId, token);
            return NoContent();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody] UpdateCommentDto commentDto, [FromHeader(Name = "X-User-ID")] Guid requestUserId, 
            CancellationToken token = default)
        {
            await _service.UpdateCommentAsync(commentDto, requestUserId, token);
            return NoContent();
        }

    }
}
