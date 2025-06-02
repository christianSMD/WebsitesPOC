using Eridian_Websites.Models.Products;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eridian_Websites.Models.Files;

public class ProductImages
{
    public int CatalogId { get; set; }
    public string ProductSku { get; set; }
    public string ThumbnailPath { get; set; }
    public bool MainImage { get; set; }
    public string TypeName { get; set; }
    public string TypeDescription { get; set; }
    public string PublicFileId { get; set; }
    public int ValueOrder { get; set; }
    [NotMapped]
    public Product Product { get; set; }
}
