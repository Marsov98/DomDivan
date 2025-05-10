namespace DomDivan.Models;

public class ProductShortInfo
{
    public int ProductId { get; set; }

    public string ProductCategory { get; set; }
    public string ProductName { get; set; }
    public string ProductTitle { 
        get 
        {
            return $"{ProductCategory} \"{ProductName}\"";
        } 
    }

    public List<VariantShortInfo> SupplyVariants { get; set; }
}
