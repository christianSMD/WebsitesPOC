using Eridian_Websites.Models;
using Eridian_Websites.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eridian_Websites.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebsitesController : ControllerBase
{
    private readonly NucleusService _nucleusService;

    public WebsitesController(NucleusService nucleusService)
    {
        _nucleusService = nucleusService;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] Website website)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { success = false, message = "Invalid data submitted." });
        }

        var result = await _nucleusService.AddWebsiteAsync(website);

        if (result.Success)
        {
            return Ok(new { success = true, message = result.Message });
        }

        return BadRequest(new { success = false, message = result.Message });
    }
}
