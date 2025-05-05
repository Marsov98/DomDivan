using DocumentFormat.OpenXml.Spreadsheet;

namespace DomDivan.Models;

public class Supply
{
    public int Id { get; set; }

    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; }

    public DateTime SupplyDate { get; set; }

    public List<ProductInSupply> ProductInSupply { get; set; } = new();
    public virtual decimal TotalPrice => ProductInSupply.Sum(i => i.Price);
}
