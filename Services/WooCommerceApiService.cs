using System.Net.Http.Headers;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Eridian_Websites.Models;
using Eridian_Websites.Models.Products;

namespace Eridian_Websites.Services
{
    public class WooCommerceApiService
    {
        private readonly HttpClient _httpClient;

        public WooCommerceApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> PushProductsAsync(Website website, List<Product> websiteProducts)
        {
            var authString = $"{website.ConsumerKey}:{website.ConsumerSecret}";
            var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes(authString));
            var baseUrl = $"{website.WebsiteURL}/wp-json/wc/v3/products/batch";

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
            string lastResponseBody = "";

            const int batchSize = 50;
            int totalBatches = (int)Math.Ceiling((double)websiteProducts.Count / batchSize);

            for (int i = 0; i < totalBatches; i++)
            {
                var batch = websiteProducts.Skip(i * batchSize).Take(batchSize).ToList();

                var createList = new List<object>();
                var updateList = new List<object>();

                foreach (var product in batch)
                {
                    // Check if product exists
                    var searchResponse = await _httpClient.GetAsync($"{website.WebsiteURL}/wp-json/wc/v3/products?sku={product.Sku}");

                    if (!searchResponse.IsSuccessStatusCode)
                    {
                        var content = await searchResponse.Content.ReadAsStringAsync();
                        throw new Exception($"Search failed: {searchResponse.StatusCode} - {content}");
                    }

                    var searchContent = await searchResponse.Content.ReadAsStringAsync();
                    var existingProducts = JsonConvert.DeserializeObject<List<dynamic>>(searchContent);
                    bool productExists = existingProducts != null && existingProducts.Count > 0;

                    if (!product.IsActive)
                    {
                        if (productExists)
                        {
                            updateList.Add(new
                            {
                                id = (int)existingProducts[0].id,
                                status = "draft"
                            });
                        }
                        continue;
                    }

                    var productData = new
                    {
                        name = product.Name,
                        type = "simple",
                        regular_price = 99.99m.ToString("F2"),
                        description = product.Description,
                        sku = product.Sku,
                        manage_stock = true,
                        stock_quantity = 0
                    };

                    if (productExists)
                    {
                        updateList.Add(new
                        {
                            id = (int)existingProducts[0].id,
                            name = product.Name,
                            type = "simple",
                            regular_price = 99.99m.ToString("F2"),
                            description = product.Description,
                            sku = product.Sku,
                            manage_stock = true,
                            stock_quantity = 0
                        });
                    }
                    else
                    {
                        var images = product.Images?.Select(img => new
                        {
                            src = $"https://nucleus.smdtechnologies.com/product-images/{img.PublicFileId}/{img.ValueOrder}.jpg"
                        }).ToList();

                        createList.Add(new
                        {
                            name = product.Name,
                            type = "simple",
                            regular_price = 99.99m.ToString("F2"),
                            description = product.Description,
                            sku = product.Sku,
                            manage_stock = true,
                            stock_quantity = 0,
                            images = images
                        });
                    }
                }

                var batchPayload = new
                {
                    create = createList,
                    update = updateList
                };

                var json = JsonConvert.SerializeObject(batchPayload,
                    new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                var contentPayload = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(baseUrl, contentPayload);
                lastResponseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Batch error: {response.StatusCode} - {lastResponseBody}");
                }
            }

            return lastResponseBody;
        }

        public async Task<string> CreateOrUpdateProductAsync(string name, string description, decimal price, int stock, string imageUrl, string sku)
        {
            // Step 1: Check if a product with the given SKU already exists
            var _baseUrl = "";
            var searchResponse = await _httpClient.GetAsync($"{_baseUrl}products?sku={sku}");
            var searchContent = await searchResponse.Content.ReadAsStringAsync();

            if (!searchResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Search failed: {searchResponse.StatusCode} - {searchContent}");
            }

            var existingProducts = JsonConvert.DeserializeObject<List<dynamic>>(searchContent);
            bool productExists = existingProducts != null && existingProducts.Count > 0;

            // Common payload
            var product = new
            {
                name = name,
                type = "simple",
                regular_price = price.ToString("F2"),
                description = description,
                sku = sku,
                manage_stock = true,
                stock_quantity = stock,
                images = new[]
                {
                    new { src = imageUrl }
                }
            };

            var json = JsonConvert.SerializeObject(product);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response;
            string responseBody;

            if (productExists)
            {
                // Step 2: Update existing product
                string productId = existingProducts[0].id.ToString();
                response = await _httpClient.PutAsync($"{_baseUrl}products/{productId}", content);
            }
            else
            {
                // Step 3: Create new product
                response = await _httpClient.PostAsync($"{_baseUrl}products", content);
            }

            responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error: {response.StatusCode} - {responseBody}");
            }

            return responseBody;
        }
    }

}
