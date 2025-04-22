namespace DomDivan.Models;

public class Bed
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string SleepingDimensions { get; set; }
}
