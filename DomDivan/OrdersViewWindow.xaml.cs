﻿using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для OrdersViewWindow.xaml
/// </summary>
public partial class OrdersViewWindow : Window, INotifyPropertyChanged
{
    private readonly DomDivanContext _context;
    private List<OrderView> _allOrders;
    private int _lowStockItemsCount;

    public int LowStockItemsCount
    {
        get => _lowStockItemsCount;
        set
        {
            _lowStockItemsCount = value;
            OnPropertyChanged();
        }
    }

    public OrdersViewWindow()
    {
        InitializeComponent();
        _context = new DomDivanContext();
        DataContext = this;
        Loaded += async (s, e) => LoadData();
    }

    private void LoadData()
    {
        try
        {
            // Загрузка заказов
            _allOrders = _context.Orders
                .AsNoTracking()
                .Include(i => i.Items)
                    .ThenInclude(i=>i.Variant)
                    .ThenInclude(v=>v.Product)
                    .ThenInclude(p=>p.Category)
                .Include(i => i.Items)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.Color)
                .Include(i => i.Items)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.Cloth)
                .Include(i => i.Items)
                    .ThenInclude(p => p.Variant)
                    .ThenInclude(v => v.SofaType)
                .AsEnumerable()
                .Select(o => new OrderView
                {
                    OrderId = o.Id,
                    CustomerName = o.CustomerName,
                    CustomerPhone = o.PhoneNumber,
                    DeliveryAddress = o.DeliveryAddress,
                    DeliveryTime = o.DeliveryDate,
                    Status = o.Status,
                    TotalPrice = o.Items.Sum(oi => oi.UnitPrice * oi.Quantity),
                    ItemsCount = o.Items.Sum(oi => oi.Quantity),
                    Items = o.Items.Select(oi => new OrderItemView
                    {
                        ProductTitle = $"{oi.Variant.Product.Category.Name} \"{oi.Variant.Product.Name}\"",
                        VariantTitle = oi.Variant.SofaType == null ?
                                        $"{oi.Variant.Color.Name} {oi.Variant.Cloth.Name}" :
                                        $"{oi.Variant.Color.Name}, {oi.Variant.Cloth.Name}, {oi.Variant.SofaType.Name}",
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        TotalPrice = oi.UnitPrice * oi.Quantity
                    })
                .ToList()
        })
                .ToList();

            // Загрузка количества товаров с малым запасом
            LowStockItemsCount = _context.Variants
                .Count(v => v.StockQuantity < 5);

            ApplyFiltersAndSort();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ApplyFiltersAndSort()
    {
        if (_allOrders == null) return;

        var filtered = _allOrders.AsQueryable();

        // Фильтрация по статусу
        if (StatusFilterComboBox.SelectedIndex > 0)
        {
            var status = (StatusFilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            filtered = filtered.Where(o => o.Status == status);
        }

        // Поиск
        if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            var searchText = SearchTextBox.Text.ToLower();
            filtered = filtered.Where(o =>
                o.CustomerName.ToLower().Contains(searchText) ||
                o.OrderId.ToString().Contains(searchText));
        }

        // Сортировка
        switch (SortComboBox.SelectedIndex)
        {
            case 0: // По времени доставки (сначала ранние)
                filtered = filtered.OrderBy(o => o.DeliveryTime);
                break;
            case 1: // По времени доставки (сначала поздние)
                filtered = filtered.OrderByDescending(o => o.DeliveryTime);
                break;
            case 2: // По сумме (↑)
                filtered = filtered.OrderBy(o => o.TotalPrice);
                break;
            case 3: // По сумме (↓)
                filtered = filtered.OrderByDescending(o => o.TotalPrice);
                break;
            default:
                filtered = filtered.OrderByDescending(o => o.DeliveryTime);
                break;
        }

        OrdersListView.ItemsSource = filtered.ToList();
    }

    private void FilterChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersAndSort();
    }

    private void SearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFiltersAndSort();
    }

    private void SortChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersAndSort();
    }

    private void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        LoadData();
    }

    private void NotificationIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        new LowStockNotificationWindow().ShowDialog();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void ViewSelectedOrder_Click(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (sender as ListBox).SelectedItem as OrderView;

        if (selectedItem != null)
        {
            new OrdersDetailsViewWindow(selectedItem).ShowDialog();
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {

    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {

    }
}