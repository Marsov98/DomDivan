namespace DomDivan.Models;

public class PhotoProduct
{
    public int Id { get; set; }

    public int VariantId { get; set; }
    public Variant Variant { get; set; }

    public string PhotoName { get; set; }
    public bool IsPrimary { get; set; }
}
