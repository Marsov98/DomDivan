using DomDivan.Models;
using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для OrdersDetailsViewWindow.xaml
/// </summary>
public partial class OrdersDetailsViewWindow : Window
{
    private readonly OrderView _order;
    private readonly DomDivanContext _context;

    public OrdersDetailsViewWindow(OrderView order)
    {
        InitializeComponent();
        _order = order;
        _context = new DomDivanContext();
        DataContext = _order;

        ItemsListView.ItemsSource = order.Items;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var order = _context.Orders.FirstOrDefault(o => o.Id == _order.OrderId);
            if (order != null)
            {
                order.Status = _order.Status;
                order.DeliveryDate = _order.DeliveryDate;
                order.Comments = _order.Comments;

                _context.Orders.Update(order);

                //Возврат товара
                if(order.Status == "Отменен")
                {
                    foreach (var item in _order.Items)
                    {
                        var orderItem = _context.Variants.FirstOrDefault(v => v.Id == item.VariantId);
                        orderItem.StockQuantity += item.Quantity;
                        _context.Variants.Update(orderItem);
                    }
                }
                _context.SaveChanges();
                MessageBox.Show("Изменения сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }
}
