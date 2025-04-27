using DomDivan.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для CartWindow.xaml
/// </summary>
public partial class CartWindow : Window
{
    public CartWindow()
    {
        InitializeComponent();
        DataContext = this;
        RefreshCart();
    }

    public ObservableCollection<CartItem> Items { get; } = new ObservableCollection<CartItem>();
    public decimal Total => CartService.Instance.Total;

    private void RefreshCart()
    {
        Items.Clear();
        foreach (var item in CartService.Instance.CartItems)
        {
            Items.Add(item);
        }
        OnPropertyChanged(nameof(Total));
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is CartItem item)
        {
            CartService.Instance.RemoveFromCart(item.Variant.Id);
            RefreshCart();
        }
    }

    private void Checkout_Click(object sender, RoutedEventArgs e)
    {
        int orderId = CartService.Instance.CreateOrder();
        MessageBox.Show($"Заказ #{orderId} оформлен на сумму {Total:C}", "Заказ оформлен");
        CartService.Instance.ClearCart();
        RefreshCart();
        Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
