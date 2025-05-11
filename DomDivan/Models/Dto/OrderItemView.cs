namespace DomDivan.Models;

public class OrderItemView
{
    public string ProductTitle { get; set; }
    public string VariantTitle { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}
