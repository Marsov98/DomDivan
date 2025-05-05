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
    private List<Supply> _allSupplies = new List<Supply>();

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
                s.Id.ToString().Contains(searchText) ||
                s.Supplier.CompanyName.ToLower().Contains(searchText) ||
                s.Supplier.ContactPerson.ToLower().Contains(searchText) ||
                s.Supplier.PhoneNumber.Contains(searchText));
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
        this.Close();
    }

    private void ViewSupplierButton_Click(object sender, RoutedEventArgs e)
    {
        new SupplierViewWindow().Show();
        this.Close();
    }
}
