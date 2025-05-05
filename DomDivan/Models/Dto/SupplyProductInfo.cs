namespace DomDivan.Models;

public class SupplyProductInfo
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

    public List<SupplyVariantInfo> SupplyVariants { get; set; }
}
