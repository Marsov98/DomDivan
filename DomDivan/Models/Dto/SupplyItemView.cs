namespace DomDivan.Models;

public class SupplyItemView
{
    public int ProductId { get; set; }
    public string ProductTitle { get; set; }

    public int VariantId { get; set; }
    public string VariantTitle { get; set; }

    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public virtual decimal TotalPrice
    {
        get
        {
            return Quantity * Price;
        }
    }
}
