namespace DomDivan.Models;

public class Armchair
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int FillerId { get; set; }
    public Filler Filler { get; set; }

    public bool HasLinenDrawer { get; set; }
    public bool HasArmrests { get; set; }
}
