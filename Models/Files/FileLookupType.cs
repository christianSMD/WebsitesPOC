namespace Eridian_Websites.Models.Files;

public class FileLookupType
{
    public int Id { get; set; }
    public string ListName { get; set; } = null!;
    public string ListValue { get; set; } = null!;
    public int? ValueOrder { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; }
    public string? Reference { get; set; }
}
