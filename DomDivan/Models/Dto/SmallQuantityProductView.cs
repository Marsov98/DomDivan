namespace DomDivan.Models;

class SmallQuantityProductView
{
    public string ProductTitle { get; set; }
    public string VariantTitle { get; set; }
    public int StockQuantity { get; set; }

    public List<SupplierShortLastInfo> SupplierShortLastInfos { get; set; }
}
