using Eridian_Websites.Models.Products;

namespace Eridian_Websites.Models.Catalogs;

public class CatalogProduct
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public int CatalogId { get; set; }
    public int ProductId { get; set; }
    public decimal MinimumPrice { get; set; }
    public decimal OriginalPrice { get; set; }
    public int? NoteLookupId { get; set; }
    public DateTime UpdatedDate { get; set; }
    public int UpdatedBy { get; set; }
    public virtual Product? Product { get; set; }
    public virtual Catalog Catalog { get; set; } = null!;
}
