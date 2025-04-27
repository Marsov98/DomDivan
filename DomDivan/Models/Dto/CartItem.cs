namespace DomDivan.Models;

public class CartItem
{
    public Variant Variant { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice => Variant.Product.DiscountPrice * Quantity;
}
