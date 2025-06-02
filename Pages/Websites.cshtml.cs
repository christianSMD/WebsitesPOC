using Eridian_Websites.Models;
using Eridian_Websites.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Eridian_Websites.Pages
{
    public class WebsitesModel : PageModel
    {
        private readonly ILogger<WebsitesModel> _logger;
        private readonly NucleusService _nucleusService;
        private readonly WooCommerceApiService _wooService;

        public List<Website> Websites { get; set; } = new();

        public WebsitesModel( ILogger<WebsitesModel> logger, NucleusService nucleusService, WooCommerceApiService wooService)
        {
            _logger = logger;
            _nucleusService = nucleusService;
            _wooService = wooService;
        }

        public async Task OnGetAsync()
        {
            Websites = await _nucleusService.GetWebsitesAsync();
        }

        public async Task<IActionResult> OnPostPushProductAsync(string name)
        {
            try
            {
                await _wooService.CreateOrUpdateProductAsync(
                    name,
                    "Sample Description",
                    99.99m,
                    10,
                    "https://via.placeholder.com/150",
                    "SKU_" + Guid.NewGuid().ToString("N").Substring(0, 8)
                );
                TempData["Message"] = $"Product '{name}' pushed successfully.";
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error pushing product '{name}': {ex.Message}";
            }

            return RedirectToPage();
        }
    }
}
