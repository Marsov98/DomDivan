namespace DomDivan.Models;

public class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; }
    public string Dimensions { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int? Discount { get; set; }

    public decimal DiscountPrice => Discount.HasValue
                    ? Price * (1 - Discount.Value / 100m)
                    : Price;

    public List<Variant> Variants { get; set; } = new();
}
