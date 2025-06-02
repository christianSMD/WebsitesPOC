namespace Eridian_Websites.Models.Catalogs;

public class Catalog
{
    public int Id { get; set; }
    public bool? IsActive { get; set; }
    public Guid PublicId { get; set; }
    public string? CatalogCode { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int CurrencyLookupId { get; set; }
    public int? NoteLookupId { get; set; }
    public int? CatalogTypeID { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int UpdatedBy { get; set; }
    public int? MarginPercentage { get; set; }
    public int? ParentId { get; set; }
    public ChannelListing? ChannelListing { get; set; }
    public virtual ICollection<CatalogProduct> CatalogProducts { get; set; } = new List<CatalogProduct>();
}
