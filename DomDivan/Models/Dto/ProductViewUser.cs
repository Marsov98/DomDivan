using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    private bool _isInCart;
    public bool IsInCart
    {
        get => _isInCart;
        set
        {
            _isInCart = value;
        }
    }

    private int _cartQuantity;
    public int CartQuantity
    {
        get => _cartQuantity;
        set
        {
            _cartQuantity = value;
        }
    }

    public string Photo { get; set; } = "/Image/picture.png";
    public virtual string? imageDirPath
    {
        get
        {
            return System.IO.Path.Combine(Environment.CurrentDirectory, $"images/{Photo}");
        }
    }

    public int StockQuantity { get; set; }
    public bool OutOfStock { get; set; } = false;
}
