using Eridian_Websites.Models.Catalogs;
using Eridian_Websites.Models.Files;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eridian_Websites.Models.Products;

public class Product
{
    public int Id { get; set; }
    public bool IsActive { get; set; }
    public Guid PublicId { get; set; }
    public int? ParentProductId { get; set; }
    public string Sku { get; set; } = null!;
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public string? Specifications { get; set; }
    public string? BoxContents { get; set; }
    public string? WarningLabels { get; set; }
    public string? KeySpecifications { get; set; }
    public string? GTINCode { get; set; }
    public string? NRCS { get; set; }
    public string? ICASACode { get; set; }
    public int CategoryId { get; set; }
    public string? Hscode { get; set; }
    public bool HasOptions { get; set; }
    public bool IsSample { get; set; }
    public bool IsPackaging { get; set; }
    public bool IsSellable { get; set; }
    public bool IsEndOfLife { get; set; }
    public bool IsVerified { get; set; }
    public int? ThumbnailFileId { get; set; }
    public bool IsExclusive { get; set; }
    public DateTime UpdatedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    [NotMapped]
    public virtual ICollection<ProductImages> Images { get; set; } = new List<ProductImages>();
    public virtual ICollection<Models.Files.File> Files { get; set; } = new List<Models.Files.File>();
}
