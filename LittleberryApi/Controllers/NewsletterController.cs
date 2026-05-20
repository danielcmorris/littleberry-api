using LittleberryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LittleberryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NewsletterController : ControllerBase
{
    private readonly IDataLayerService _dataLayer;

    public NewsletterController(IDataLayerService dataLayer)
    {
        _dataLayer = dataLayer;
    }

    /// <summary>
    /// Get newsletters
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var sql = "SELECT * FROM Newsletters ORDER BY IssueDate DESC";
        var ds = await _dataLayer.GetDataAsync(sql);

        if (ds.Tables.Count > 0)
        {
            var newsletters = new List<object>();
            foreach (System.Data.DataRow row in ds.Tables[0].Rows)
            {
                newsletters.Add(new
                {
                    Id = row["Id"],
                    Title = row["Title"],
                    IssueDate = row["IssueDate"],
                    Url = row["Url"]
                });
            }
            return Ok(newsletters);
        }

        return Ok(new List<object>());
    }
}
