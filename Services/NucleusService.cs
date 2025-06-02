using Eridian_Websites.Data;
using Eridian_Websites.Models;
using Eridian_Websites.Models.Catalogs;
using Eridian_Websites.Models.Products;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace Eridian_Websites.Services
{
    public class NucleusService
    {
        private readonly NucleusDbContext _context;

        public NucleusService(NucleusDbContext context)
        {
            _context = context;
        }

        public async Task<List<Website>> GetWebsitesAsync()
        {
            return await _context.Catalogs
                .Where(c => c.IsActive == true && c.ChannelListing != null && c.ChannelListing.IsActive == true)
                .Select(c => new Website
                {
                    CatalogId = c.Id,
                    CatalogPublicId = c.PublicId,
                    Name = c.Name,
                    WebsiteURL = c.ChannelListing!.WebsiteURL,
                    Description = c.Description,
                    ConsumerKey = c.ChannelListing.ConsumerKey,
                    ConsumerSecret = c.ChannelListing.ConsumerSecret
                })
                .ToListAsync();
        }

        public async Task<List<Product>> GetCatalogProductsAsync(int catalogId)
        {
            var products = await _context.Catalogs
                .Where(c => c.Id == catalogId)
                .Include(c => c.CatalogProducts)
                    .ThenInclude(cp => cp.Product)
                .SelectMany(c => c.CatalogProducts.Select(cp => cp.Product))
                .ToListAsync();

            var skus = products.Select(p => p.Sku).ToList();

            var images = await _context.ProductImages
                .Where(img => skus.Contains(img.ProductSku))
                .ToListAsync();

            // Manually assign images
            foreach (var product in products)
            {
                product.Images = images.Where(img => img.ProductSku == product.Sku).ToList();
            }

            return products;
        }

        public async Task<OperationResult> AddWebsiteAsync(Website website)
        {
            try
            {
                var lookupTypeId = await _context.CatalogLookupTypes
                    .Where(lt => lt.ListValue == "Channel Listing")
                    .Select(lt => lt.Id)
                    .FirstOrDefaultAsync();

                if (lookupTypeId == 0)
                    return new OperationResult { Success = false, Message = "Catalog lookup type not found." };

                var newCatalog = new Catalog
                {
                    Name = website.Name,
                    PublicId = Guid.NewGuid(),
                    Description = website.Description,
                    CatalogTypeID = lookupTypeId,
                    IsActive = true,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = 29
                };

                _context.Catalogs.Add(newCatalog);
                await _context.SaveChangesAsync(); // Save catalog to get ID

                var newChannelListing = new ChannelListing
                {
                    CatalogId = newCatalog.Id,
                    ConsumerKey = website.ConsumerKey,
                    ConsumerSecret = website.ConsumerSecret,
                    WebsiteURL = website.WebsiteURL,
                    IsActive = true,
                    UpdatedDate = DateTime.UtcNow,
                    UpdatedBy = 29
                };

                _context.ChannelListings.Add(newChannelListing);
                await _context.SaveChangesAsync();

                return new OperationResult { Success = true, Message = "Website and catalog created successfully." };
            }
            catch (Exception ex)
            {
                // Log exception (optional)
                // _logger.LogError(ex, "Failed to add website");

                return new OperationResult
                {
                    Success = false,
                    Message = "An error occurred while creating the website."
                };
            }
        }
    }

    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
