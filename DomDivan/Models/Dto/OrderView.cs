namespace DomDivan.Models;

public class OrderView
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string DeliveryAddress { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int ItemsCount { get; set; }

    public List<OrderItemView> Items { get; set; } = new();
}
