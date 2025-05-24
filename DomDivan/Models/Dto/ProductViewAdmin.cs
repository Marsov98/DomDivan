namespace DomDivan.Models;

public class ProductViewAdmin
{
    public int ProductId { get; set; }

    public string Category { get; set; } = "Без категории";
    public string Name { get; set; } = "Без названия";
    public string FullName => $"{Category} \"{Name}\"";

    public decimal Price { get; set; }
    public int? Discount { get; set; }
    public decimal DiscountPrice => Discount.HasValue
        ? Price * (1 - Discount.Value / 100m)
        : Price;

    public string? Filler { get; set; }
    public string? Mechanism { get; set; }
    public virtual string? Description
    {
        get
        {
            var parts = $"";
            if (!string.IsNullOrEmpty(Filler))
            {
                parts += Filler;
            }
            if (!string.IsNullOrEmpty(Mechanism))
            {
                parts += ", " + Mechanism;
            }
            return parts;
        }
    }

    public List<Variant> Variants { get; set; } = new();
    public virtual int? NotificationCount => Variants.Count(v => v.StockQuantity < 5) == 0 
                                             ? null :
                                             Variants.Count(v => v.StockQuantity < 5);
}
