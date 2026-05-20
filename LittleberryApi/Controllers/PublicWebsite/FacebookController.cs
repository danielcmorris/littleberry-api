using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LittleberryApi.Controllers.PublicWebsite;

[ApiController]
public class FacebookController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<FacebookController> _logger;

    public FacebookController(IHttpClientFactory httpClientFactory, ILogger<FacebookController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Get Facebook posts from a page
    /// </summary>
    [HttpGet]
    [Route("api/facebook")]
    public async Task<IActionResult> GetFacebookPosts([FromQuery] string pageId, [FromQuery] string accessToken)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://graph.facebook.com/{pageId}/posts?access_token={accessToken}&fields=message,created_time,full_picture,permalink_url";

            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Failed to fetch Facebook posts");
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            return Ok(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Facebook posts");
            return StatusCode(500, "Error fetching Facebook posts");
        }
    }
}
