using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IAwsS3Service _s3Service;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IAwsS3Service s3Service, ILogger<ImageController> logger)
    {
        _s3Service = s3Service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Post()
    {
        try
        {
            var files = Request.Form.Files;

            if (files.Count == 0)
                return BadRequest("No files uploaded");

            var file = files[0];
            var extension = Path.GetExtension(file.FileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Allowed types: jpg, jpeg, png, gif, bmp");

            var callNumber = Request.Form["callnumber"].ToString().Trim();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddTHHmmss");
            var key = string.IsNullOrEmpty(callNumber)
                ? $"{timestamp}{extension}"
                : $"{callNumber}/{timestamp}{extension}";

            using var stream = file.OpenReadStream();
            await _s3Service.UploadFileAsync(stream, key, file.ContentType);

            return Ok(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, "Error uploading image");
        }
    }
}
