using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для MainMenuWindow.xaml
/// </summary>
public partial class MainMenuWindow : Window
{
    public MainMenuWindow()
    {
        InitializeComponent();
    }

    private void SuppliesButton_Click(object sender, RoutedEventArgs e)
    {
        var suppliesWindow = new SuppliesViewWindow();
        suppliesWindow.Show();
        this.Close();
    }

    private void OrdersButton_Click(object sender, RoutedEventArgs e)
    {
        /*var ordersWindow = new OrdersViewWindow();
        ordersWindow.ShowDialog();*/
    }

    private void ProductsButton_Click(object sender, RoutedEventArgs e)
    {
        var productsWindow = new AdminProductsWindow(); // Замените на ваше окно товаров
        productsWindow.Show();
        this.Close();
    }
}
