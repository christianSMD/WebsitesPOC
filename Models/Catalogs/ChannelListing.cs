namespace Eridian_Websites.Models.Catalogs;

public class ChannelListing
{
    public int Id { get; set; }
    public int CatalogId { get; set; }
    public bool? IsActive { get; set; }
    public string? ConsumerKey { get; set; }
    public string? ConsumerSecret { get; set; }
    public string? WebsiteURL { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int UpdatedBy { get; set; }
    public Catalog Catalog { get; set; } = null!;
}
