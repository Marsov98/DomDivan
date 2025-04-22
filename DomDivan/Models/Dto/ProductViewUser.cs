namespace DomDivan.Models;

public class ProductViewUser
{
    public int IdVariant { get; set; }

    public string Category { get; set; } = "Без категории";
    public string Name { get; set; } = "Без названия";
    public string FullName => $"{Category} \"{Name}\"";

    public decimal Price { get; set; }
    public int? Discount { get; set; }
    public decimal DiscountPrice => Discount.HasValue
        ? Price * (1 - Discount.Value / 100m)
        : Price;

    public string Color { get; set; }
    public string Cloth { get; set; }
    public string? Filler { get; set; }
    public string? Mechanism { get; set; }
    public virtual string? Description 
    { 
        get 
        {
            var parts = $"{Color}, {Cloth}";
            if (!string.IsNullOrEmpty(Filler))
            {
                parts += ", " + Filler ;
            }
            if (!string.IsNullOrEmpty(Mechanism))
            {
                parts += ", " + Mechanism;
            }
            return parts;
        } 
    }


    public string Photo { get; set; } = "default.jpg";
    public virtual string? ImagePath
    {
        get
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, $"images/{Photo}");
        }
    }

    public int StockQuantity { get; set; }
}
