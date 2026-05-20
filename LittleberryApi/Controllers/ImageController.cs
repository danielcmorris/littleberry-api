using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly IAzureBlobService _blobService;
    private readonly ILogger<ImageController> _logger;

    public ImageController(IAzureBlobService blobService, ILogger<ImageController> logger)
    {
        _blobService = blobService;
        _logger = logger;
    }

    /// <summary>
    /// Upload an image
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Post()
    {
        try
        {
            var files = Request.Form.Files;

            if (files.Count == 0)
            {
                return BadRequest("No files uploaded");
            }

            var file = files[0];
            var fileName = file.FileName;

            // Get the file extension
            var extension = Path.GetExtension(fileName).ToLower();
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest("Invalid file type. Allowed types: jpg, jpeg, png, gif, bmp");
            }

            // Generate a unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            using var stream = file.OpenReadStream();
            var url = await _blobService.UploadImageAsync(stream, uniqueFileName, file.ContentType);

            return Ok(new { url, fileName = uniqueFileName });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, "Error uploading image");
        }
    }
}
