namespace DomDivan.Models;

public class CartView
{
    public int IdVariant { get; set; }
    public string Title { get; set; }
    public string PrimaryPhoto { get; set; }
    public string Color { get; set; }
    public string Cloth { get; set; }
    public string? SofaType { get; set; }
    public decimal Price { get; set; }
    public int? Discount { get; set; }
    public decimal DiscountPrice { get; set; }
    public int Quantity { get; set; }
}
