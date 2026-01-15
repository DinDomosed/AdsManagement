using AdsManagement.API.Adapters;
using AdsManagement.API.Requests;
using AdsManagement.API.Responses;
using AdsManagement.App.Common;
using AdsManagement.App.DTOs.Advertisement;
using AdsManagement.App.Interfaces;
using AdsManagement.App.Interfaces.Service;
using AdsManagement.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdsManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _service;

        public AdvertisementController(IAdvertisementService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvertisment([FromRoute] Guid id, CancellationToken token = default)
        {
            var ad = await _service.GetAdvertisementAsync(id, token);
            var response = GetApiResponse(ad);
            return Ok(response);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAdvertisement([FromForm] CreateAdvertisementRequest request,
            CancellationToken token = default)
        {
            IFileData fileData = null;
            if (request.FormFile is not null)
            {
                fileData = new FormFileDataAdapter(request.FormFile);
            }
            var createDto = new CreateAdvertisementDto()
            {
                UserId = request.UserId,
                Text = request.Text,
                Title = request.Title
            };

            var adId = await _service.AddAdvertisementAsync(createDto, fileData, token);
            var adDb = await _service.GetAdvertisementAsync(adId, token);
            var response = GetApiResponse(adDb);

            return CreatedAtAction(nameof(GetAdvertisment), new { id = adId }, response);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdvertisement([FromRoute] Guid id, [FromHeader(Name = "X-User-ID")] Guid requestUserId, CancellationToken token = default)
        {
            await _service.DeleteAdvertisementAsync(id, requestUserId, token);
            return NoContent();
        }
        [HttpPut]
        public async Task<ActionResult> UpdateAdvertisement([FromBody] UpdateAdvertisementDto advertisementDto, [FromHeader(Name = "X-User-ID")] Guid requestUserId,
            CancellationToken token = default)
        {
            await _service.UpdateAdvertisementAsync(advertisementDto, requestUserId, token);
            return NoContent();
        }
        [HttpGet("search")]
        public async Task<IActionResult> GetByFilter([FromQuery] AdFilterDto filter, CancellationToken token = default)
        {
            var paged = await _service.GetFilterAdsAsync(filter, token);
            var response = GetApiResponse(paged);
            return Ok(response);
        }
        [HttpGet("search/byuser/{userId}")]
        public async Task<IActionResult> GetByUser([FromRoute] Guid userId, [FromQuery] UserAdvertisementFilterDto filter, CancellationToken token = default)
        {
            var paged = await _service.GetByUserAdsAsync(userId, filter, token);
            var response = GetApiResponse(paged);
            return Ok(response);
        }
        private ResponseAdApiDto GetApiResponse(ResponseAdvertisementDto dto)
        {
            return new ResponseAdApiDto
            {
                Id = dto.Id,
                UserId = dto.UserId,
                Number = dto.Number,
                Title = dto.Title,
                Text = dto.Text,
                Rating = dto.Rating,
                CreatedAt = dto.CreatedAt,
                ExpiresAt = dto.ExpiresAt,
                Images = dto.Images == null || dto.Images.Count == 0
                ? new List<ResponseAdImageApiDto>()
                : dto.Images.Select(image => new ResponseAdImageApiDto
                {
                    Id = image.Id,
                    AdvertisementId = image.AdvertisementId,
                    ImageURL = Url.Action(nameof(AdImageController.GetAdImage), "AdImage", new { id = image.Id }),
                    SmallImageURL = Url.Action(nameof(AdImageController.GetAdImageSmall), "AdImage", new { id = image.Id })
                }).ToList()
            };
        }
        private PagedResult<ResponseAdApiDto> GetApiResponse(PagedResult<ResponseAdvertisementDto> paged)
        {
            var responsePaged = new PagedResult<ResponseAdApiDto>
            {
                Items = paged.Items.Select(dto => GetApiResponse(dto)).ToList(),
                Page = paged.Page,
                PageSize = paged.PageSize,
                TotalCount = paged.TotalCount
            };
            return responsePaged;
        }
    }
}
