using Eridian_Websites.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml.Linq;

namespace Eridian_Websites.Pages
{
    public class TestWordPressDBModel : PageModel
    {
        private readonly WpDbService _wpDb;
        private readonly WooCommerceApiService _woCommerceApiService;

        public List<string> ProductTitles { get; set; } = new();

        public TestWordPressDBModel(WpDbService wpDb, WooCommerceApiService woCommerceApiService    )
        {
            _wpDb = wpDb;
            _woCommerceApiService = woCommerceApiService;
        }

        public async Task OnGetAsync()
        {
            // Products from WordPress
            ProductTitles = await _wpDb.GetProductTitlesAsync();
        }

        public async Task<IActionResult> OnPostAddProductAsync()
        {
            for (int i = 1; i <= 100; i++)
            {
                string sku = $"Test-sku-{i:000}";
                string name = $"Test Product Woo {i}";

                await _woCommerceApiService.CreateOrUpdateProductAsync(
                    name: name,
                    description: "This is a test WooCommerce product inserted via Razor Pages using WooCommerce API.",
                    price: 199.99m,
                    stock: 10,
                    imageUrl: "https://nucleus.smdtechnologies.com/storefront-images/3735085E-07A6-4886-A2E5-29C635E06018/1024/1024/0.png",
                    sku: sku
                );
            }


            // WooCommerce API
            //await _woCommerceApiService.CreateOrUpdateProductAsync(
            //    name: "Test Product Woo",
            //    description: "This is a test WooCommerce product inserted via Razor Pages using WooCommerce API.",
            //    price: 199.99m,
            //    stock: 10,
            //    imageUrl: "https://nucleus.smdtechnologies.com/product-images/CA412727-E979-4923-A32A-5E4161A2378E/1.jpg",
            //    sku: "Test-sku-001"
            //);

            // Direct DB Insert
            //await _wpDb.InsertProductAsync(
            //    title: "Test Product from C#",
            //    description: "This is a test WooCommerce product inserted via Razor Pages.",
            //    price: 199.99m,
            //    stock: 10,
            //    imageUrl: "https://nucleus.smdtechnologies.com/product-images/CA412727-E979-4923-A32A-5E4161A2378E/1.jpg"
            //);

            return RedirectToPage(); // Refresh
        }

    }
}
