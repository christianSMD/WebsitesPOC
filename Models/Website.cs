using Eridian_Websites.Models.Products;

namespace Eridian_Websites.Models;

public class Website
{
    public int CatalogId { get; set; }
    public Guid CatalogPublicId { get; set; }
    public string Name { get; set; } = null!;
    public string? WebsiteURL { get; set; }
    public string? Description { get; set; }
    public string? ConsumerKey { get; set; }
    public string? ConsumerSecret { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
