using Eridian_Websites.Models.Catalogs;
using Eridian_Websites.Models.Products;

namespace Eridian_Websites.Models.Files;

public class File
{
    public int Id { get; set; }
    public Guid PublicId { get; set; }
    public string Name { get; set; }
    public string? Path { get; set; }
    public int LookupTypeId { get; set; }
    public int? LanguageLookupTypeId { get; set; }
    public int PrimaryObjectLookupId { get; set; }
    public int PrimaryObjectId { get; set; }
    public DateTime CreatedDate { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool Ecommerce { get; set; }
    public long? Size { get; set; }
    public bool? FileExists { get; set; }
    public int UpdatedBy { get; set; }
    public int CreatedBy { get; set; }
    public DateTime UpdatedDate { get; set; }
    public Product Product { get; set; } = null!;
    public FileLookupType LookupType { get; set; } = null!;
}
