using Eridian_Websites.Models;
using Eridian_Websites.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eridian_Websites.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PushController : ControllerBase
{
    private readonly NucleusService _nucleusService;
    private readonly WooCommerceApiService _wooService;

    public PushController(NucleusService nucleusService, WooCommerceApiService wooService)
    {
        _nucleusService = nucleusService;
        _wooService = wooService;
    }

    [HttpPost("product")]
    public async Task<IActionResult> PushProduct([FromBody] PushRequest request)
    {
        var website = (await _nucleusService.GetWebsitesAsync()).FirstOrDefault(w => w.CatalogId == request.CatalogId);

        if (website == null)
            return NotFound($"Website not found.");

        try
        {
            var websiteProducts = await _nucleusService.GetCatalogProductsAsync(request.CatalogId);

            if (websiteProducts == null || websiteProducts.Count < 1)
                return NotFound($"No products to push for {website.Name}.");

            await _wooService.PushProductsAsync(website, websiteProducts);

            return Ok($"Product pushed to {website.Name}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add([FromBody] Website website)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _nucleusService.AddWebsiteAsync(website);

        if (result.Success)
            return Ok(result.Message);

        return BadRequest(result.Message);
    }
}


public class PushRequest
{
    public int CatalogId { get; set; }
    public string? Name { get; set; } // Optional
}