namespace DomDivan.Models;

public class SupplyView
{
    public int SupplyId { get; set; }

    public string CompanyName { get; set; }
    public string ContactPerson { get; set; }
    public string PhoneNumber { get; set; }

    public DateTime SupplyDate { get; set; }

    public List<SupplyItemView> ProductInSupply { get; set; } = new();
    public virtual decimal TotalPrice => ProductInSupply.Sum(i => i.TotalPrice);
}
