using DomDivan.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

public partial class CartWindow : Window, INotifyPropertyChanged
{
    public ObservableCollection<CartView> CartItems { get; } = new ObservableCollection<CartView>();
    public decimal TotalPrice { get; set; }
    public decimal TotalDiscountPrice {  get; set; }
    public bool HasEqual { get; set; }
    public string imageDirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Image"));

    public CartWindow()
    {
        InitializeComponent();
        DataContext = this;
        LoadCartItems();
    }

    private void LoadCartItems()
    {
        CartItems.Clear();

        // Здесь получаем данные из CartService и преобразуем в CartView
        foreach (var cartItem in CartService.Instance.CartItems)
        {
            CartItems.Add(new CartView
            {
                VariantId = cartItem.Variant.Id,
                Title = $"{cartItem.Variant.Product.Category.Name} \"{cartItem.Variant.Product.Name}\"",
                PrimaryPhoto = imageDirPath + '\\' + cartItem.Variant.Photos.FirstOrDefault(p => p.IsPrimary).PhotoName,
                Color = cartItem.Variant.Color.Name,
                Cloth = cartItem.Variant.Cloth.Name,
                SofaType = cartItem.Variant.SofaType?.Name,
                Price = cartItem.Variant.Product.Price,
                Discount = cartItem.Variant.Product.Discount,
                DiscountPrice = cartItem.Variant.Product.DiscountPrice,
                Quantity = cartItem.Quantity
            });
        }

        TotalPrice = CartItems.Sum(item => item.Price * item.Quantity);
        TotalDiscountPrice = CartItems.Sum(item => item.DiscountPrice * item.Quantity);

        HasEqual = TotalPrice == TotalDiscountPrice;

        OnPropertyChanged(nameof(TotalPrice));
        OnPropertyChanged(nameof(TotalDiscountPrice));
        OnPropertyChanged(nameof(HasEqual));
    }

    private void RemoveItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int variantId)
        {
            CartService.Instance.RemoveFromCart(variantId);
            LoadCartItems();
        }
    }

    private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int variantId)
        {
            int newQuantity = CartService.Instance.GetItemQuantity(variantId) - 1;
            if (newQuantity <= 0)
            {
                CartService.Instance.RemoveFromCart(variantId);
            }
            else
            {
                CartService.Instance.UpdateQuantity(variantId, newQuantity);
            }
            LoadCartItems();
        }
    }

    private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int variantId)
        {
            CartService.Instance.UpdateQuantity(variantId,
                CartService.Instance.GetItemQuantity(variantId) + 1);
            LoadCartItems();
        }
    }

    private void Checkout_Click(object sender, RoutedEventArgs e)
    {
        if (CartItems.Count == 0)
        {
            MessageBox.Show("Корзина пуста", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        new CheckoutWindow().Show();
        this.Close();

        //GenerateWord.GenerateWordReceipt(CartItems);

        /*var checkoutWindow = new CheckoutWindow();
        if (checkoutWindow.ShowDialog() == true)
        {
            int orderId = CartService.Instance.CreateOrder();
            MessageBox.Show($"Заказ #{orderId} оформлен на сумму {Total:C}", "Заказ оформлен");
            CartService.Instance.ClearCart();
            Close();
        }*/
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        this.Close();
    }

    private void ContinueShopping_Click(object sender, RoutedEventArgs e)
    {
        new CatalogWindow().Show();
        this.Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}