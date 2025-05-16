using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для SuppliesViewWindow.xaml
/// </summary>
public partial class SuppliesViewWindow : Window
{
    private readonly DomDivanContext _context;
    private List<SupplyView> _allSupplies = new List<SupplyView>();

    public SuppliesViewWindow()
    {
        InitializeComponent();
        _context = new DomDivanContext();
        LoadSupplies();
    }

    private void LoadSupplies()
    {
        try
        {
            _allSupplies = _context.Supplies
                .Include(s => s.Supplier)
                .Include(s => s.ProductInSupply)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.Product)
                    .ThenInclude(p => p.Category)
                .Include(s => s.ProductInSupply)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.Color)
                .Include(s => s.ProductInSupply)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.Cloth)
                .Include(s => s.ProductInSupply)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.SofaType)
                .Select(item => new SupplyView
                {
                    SupplyId = item.Id,
                    CompanyName = item.Supplier.CompanyName,
                    ContactPerson = item.Supplier.ContactPerson,
                    PhoneNumber = item.Supplier.PhoneNumber,
                    SupplyDate = item.SupplyDate,
                    ProductInSupply = item.ProductInSupply.Select(p => new SupplyItemView
                    {
                        ProductId = p.Variant.ProductId,
                        ProductTitle = $"{p.Variant.Product.Category.Name} \"{p.Variant.Product.Name}\"",
                        VariantId = p.Variant.Id,
                        VariantTitle = p.Variant.SofaType == null ?
                                        $"{p.Variant.Color.Name} {p.Variant.Cloth.Name}" :
                                        $"{p.Variant.Color.Name}, {p.Variant.Cloth.Name}, {p.Variant.SofaType.Name}",
                        Quantity = p.Quantity,
                        Price = p.Price
                    }).ToList()
                })
                .ToList();

            ApplySearchAndSort();
            UpdateStats();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки поставок: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplySearchAndSort()
    {
        var filtered = _allSupplies.AsQueryable();

        // Поиск
        if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            var searchText = SearchTextBox.Text.ToLower();
            filtered = filtered.Where(s =>
                s.SupplyId.ToString().Contains(searchText) ||
                s.CompanyName.ToLower().Contains(searchText) ||
                s.ContactPerson.ToLower().Contains(searchText) ||
                s.PhoneNumber.Contains(searchText));
        }

        // Сортировка
        var selectedSort = (SortComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
        switch (selectedSort)
        {
            case "По дате (новые)":
                filtered = filtered.OrderByDescending(s => s.SupplyDate);
                break;
            case "По дате (старые)":
                filtered = filtered.OrderBy(s => s.SupplyDate);
                break;
            case "По сумме (↑)":
                filtered = filtered.OrderBy(s => s.TotalPrice);
                break;
            case "По сумме (↓)":
                filtered = filtered.OrderByDescending(s => s.TotalPrice);
                break;
            default:
                filtered = filtered.OrderByDescending(s => s.SupplyDate);
                break;
        }

        SuppliesListView.ItemsSource = filtered.ToList();
    }

    private void UpdateStats()
    {
        var totalSupplies = _allSupplies.Count;
        var totalProducts = _allSupplies.Sum(s => s.ProductInSupply.Sum(p => p.Quantity));
        var totalSum = _allSupplies.Sum(s => s.TotalPrice);

        StatsTextBlock.Text = $"Всего поставок: {totalSupplies} | " +
                            $"Товаров: {totalProducts} | " +
                            $"Общая сумма: {totalSum:C}";
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplySearchAndSort();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplySearchAndSort();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadSupplies();
    }

    private void NewSupplyButton_Click(object sender, RoutedEventArgs e)
    {
        new AddSupplyWindow().Show();
    }

    private void ViewSupplierButton_Click(object sender, RoutedEventArgs e)
    {
        new SupplierViewWindow().Show();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        new MainMenuWindow().Show();
        this.Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        this.Close();
    }
}
