using DomDivan.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DomDivan;

public class CartService : INotifyPropertyChanged
{
    private static CartService _instance;
    public static CartService Instance => _instance ??= new CartService();

    private ObservableCollection<CartItem> _cartItems = new ObservableCollection<CartItem>();

    public event PropertyChangedEventHandler PropertyChanged;

    public IReadOnlyCollection<CartItem> CartItems => _cartItems.ToList().AsReadOnly();

    public decimal Total => _cartItems.Sum(item => item.TotalPrice);

    public int ItemsCount => _cartItems.Sum(item => item.Quantity);

    private CartService()
    {
        _cartItems.CollectionChanged += (s, e) => OnPropertyChanged();
    }

    public void AddToCart(Variant variant, int quantity = 1)
    {
        var existingItem = _cartItems.FirstOrDefault(item => item.Variant.Id == variant.Id);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            _cartItems.Add(new CartItem { Variant = variant, Quantity = quantity });
        }
        OnPropertyChanged();
    }

    public bool IsInCart(int variantId)
    {
        return _cartItems.Any(item => item.Variant.Id == variantId);
    }

    public bool IsMaxQuantity(int variantId, int MaxQuantity)
    {
        return MaxQuantity == GetItemQuantity(variantId);
    }

    public int GetItemQuantity(int variantId)
    {
        return _cartItems.FirstOrDefault(item => item.Variant.Id == variantId)?.Quantity ?? 0;
    }

    public void RemoveFromCart(int variantId)
    {
        var itemToRemove = _cartItems.FirstOrDefault(item => item.Variant.Id == variantId);
        if (itemToRemove != null)
        {
            _cartItems.Remove(itemToRemove);
        }
        OnPropertyChanged();
    }

    public void ClearCart()
    {
        _cartItems.Clear();
        OnPropertyChanged();
    }

    public void UpdateQuantity(int variantId, int newQuantity)
    {
        var item = _cartItems.FirstOrDefault(i => i.Variant.Id == variantId);
        if (item != null)
        {
            item.Quantity = newQuantity;
        }
        OnPropertyChanged();
    }

    public int CreateOrder()
    {
        // Здесь должна быть логика создания заказа в БД
        return 1;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}