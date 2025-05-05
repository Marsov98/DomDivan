namespace DomDivan.Models;

public class ProductInSupply
{
    public int Id { get; set; }

    public int SupplyId { get; set; }
    public Supply Supply { get; set; }

    public int VariantId { get; set; }
    public Variant Variant { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
