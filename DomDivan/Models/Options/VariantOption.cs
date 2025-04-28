namespace DomDivan.Models;

public class VariantOption<T>
{
    public T Color { get; set; }
    public T Cloth { get; set; }
    public T SofaType { get; set; }
    public bool IsSelected { get; set; }
}
