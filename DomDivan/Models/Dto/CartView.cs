namespace DomDivan.Models;

public class CartView
{
    public int VariantId { get; set; }
    public string Title { get; set; }
    public string PrimaryPhoto { get; set; }
    public string Color { get; set; }
    public string Cloth { get; set; }
    public string? SofaType { get; set; }
    public virtual string TitleVariant => SofaType != null ?
                                          $"{Color}, {Cloth}, {SofaType}" :
                                          $"{Color}, {Cloth}";
    public decimal Price { get; set; }
    public int? Discount { get; set; }
    public decimal DiscountPrice { get; set; }
    public int Quantity { get; set; }
}
