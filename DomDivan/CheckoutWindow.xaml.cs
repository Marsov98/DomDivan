using DomDivan.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DomDivan;

public partial class CheckoutWindow : Window, INotifyPropertyChanged
{
    private Order _currentOrder;
    public Order CurrentOrder
    {
        get => _currentOrder;
        set
        {
            _currentOrder = value;
            OnPropertyChanged(nameof(CurrentOrder));
            OnPropertyChanged(nameof(IsOrderValid));
        }
    }

    public bool IsOrderValid { get; private set; }

    public int TotalQuantity => CurrentOrder.Items.Sum(x => x.Quantity);

    public CheckoutWindow()
    {
        InitializeComponent();
        InitializeOrder();
        DataContext = this;
    }

    private void InitializeOrder()
    {
        CurrentOrder = new Order
        {
            DeliveryDate = DateTime.Now.AddDays(3),
            Status = "Новый",
            Items = CartService.Instance.CartItems
                .Select((item, index) => new OrderItem
                {
                    Id = index + 1,
                    VariantId = item.Variant.Id,
                    Variant = item.Variant,
                    Quantity = item.Quantity,
                    UnitPrice = item.Variant.Product.DiscountPrice
                })
                .ToList()
        };
    }

    private void Validate_TextChanged(object sender, TextChangedEventArgs e)
    {
        ValidateOrder();
    }

    private void Validate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
    {
        ValidateOrder();
    }

    private void ValidateOrder()
    {
        IsOrderValid = !string.IsNullOrWhiteSpace(CurrentOrder.CustomerName) &&
                      !string.IsNullOrWhiteSpace(CurrentOrder.PhoneNumber) &&
                      !string.IsNullOrWhiteSpace(CurrentOrder.DeliveryAddress) &&
                      CurrentOrder.DeliveryDate >= DateTime.Now.Date &&
                      CurrentOrder.Items.Any();

        OnPropertyChanged(nameof(IsOrderValid));
    }

    private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
    {
        if (!IsOrderValid)
        {
            MessageBox.Show("Пожалуйста, заполните все обязательные поля корректно.",
                          "Ошибка оформления", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            foreach(var item in CurrentOrder.Items)
            {
                item.Id = 0;
                item.Variant = null;
            }
            using(var context = new DomDivanContext())
            {
                context.Orders.Add(CurrentOrder);

                foreach (var item in CurrentOrder.Items)
                {
                    var currentItem = context.Variants.FirstOrDefault(v => v.Id == item.VariantId);
                    currentItem.StockQuantity -= item.Quantity;
                    context.Variants.Update(currentItem);
                }

                context.SaveChanges();
            }

            MessageBox.Show($"Заказ №{CurrentOrder.Id} успешно оформлен!\n\n" +
                          $"Клиент: {CurrentOrder.CustomerName}\n" +
                          $"Телефон: {CurrentOrder.PhoneNumber}\n" +
                          $"Адрес: {CurrentOrder.DeliveryAddress}\n" +
                          $"Дата доставки: {CurrentOrder.DeliveryDate:dd.MM.yyyy}\n" +
                          $"Сумма заказа: {CurrentOrder.TotalPrice:C}",
                          "Заказ оформлен", MessageBoxButton.OK, MessageBoxImage.Information);

            ObservableCollection<CartView> CartItems = new();

            foreach (var cartItem in CartService.Instance.CartItems)
            {
                CartItems.Add(new CartView
                {
                    VariantId = cartItem.Variant.Id,
                    Title = $"{cartItem.Variant.Product.Category.Name} \"{cartItem.Variant.Product.Name}\"",
                    PrimaryPhoto = cartItem.Variant.Photos.FirstOrDefault(p => p.IsPrimary)?.PhotoName
                                ?? cartItem.Variant.Photos.FirstOrDefault()?.PhotoName,
                    Color = cartItem.Variant.Color.Name,
                    Cloth = cartItem.Variant.Cloth.Name,
                    SofaType = cartItem.Variant.SofaType?.Name,
                    Price = cartItem.Variant.Product.Price,
                    Discount = cartItem.Variant.Product.Discount,
                    DiscountPrice = cartItem.Variant.Product.DiscountPrice,
                    Quantity = cartItem.Quantity
                });
            }

            GenerateWord.GenerateWordReceipt(CartItems, CurrentOrder);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", 
                            "Ошибка оформления", MessageBoxButton.OK, MessageBoxImage.Warning);
        }



        CartService.Instance.ClearCart();
        new CatalogWindow().Show();
        this.Close();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        new CartWindow().Show();
        this.Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        this.Close();
    }

    private void ContinueShoppingButton_Click(object sender, RoutedEventArgs e)
    {
        new CatalogWindow().Show();
        this.Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}