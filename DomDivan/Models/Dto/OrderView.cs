﻿namespace DomDivan.Models;

public class OrderView
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public string DeliveryAddress { get; set; }
    public DateTime DeliveryDate { get; set; }
    public DateTime CreateDate { get; set; }
    public string Status { get; set; }
    public string Comments { get; set; }
    public decimal TotalPrice { get; set; }
    public int ItemsCount { get; set; }

    public List<OrderItemView> Items { get; set; } = new();
    public virtual bool IsNotification => DeliveryDate.Date == DateTime.Today;
}
