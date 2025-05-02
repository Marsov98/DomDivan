namespace DomDivan.Models;

public class Variant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int ColorId { get; set; }
    public ColorVariant Color { get; set; }

    public int ClothId { get; set; }
    public Cloth Cloth { get; set; }

    public int? SofaTypeId { get; set; }
    public SofaType SofaType { get; set; }

    public int StockQuantity { get; set; } = 0;

    public List<PhotoProduct> Photos { get; set; } = new();
}
