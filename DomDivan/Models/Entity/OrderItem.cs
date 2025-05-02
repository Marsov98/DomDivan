namespace DomDivan.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int VariantId { get; set; }
    public Variant Variant { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public virtual decimal TotalPrice 
    { 
        get
        {
            return Quantity * UnitPrice; 
        }
    }
}
