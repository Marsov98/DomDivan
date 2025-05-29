namespace DomDivan.Models;

public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime DeliveryDate { get; set; }
    public string Status { get; set; }

    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string DeliveryAddress { get; set; }
    public string? Comments { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public List<OrderItem> Items { get; set; } = new();
    public virtual decimal TotalPrice => Items.Sum(i => i.TotalPrice);
}
