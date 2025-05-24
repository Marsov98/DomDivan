using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для AdminProductsWindow.xaml
/// </summary>
public partial class AdminProductsWindow : Window, INotifyPropertyChanged
{
    private readonly DomDivanContext _context;
    private List<ProductViewAdmin> _allProducts;

    public AdminProductsWindow()
    {
        InitializeComponent();
        _context = new DomDivanContext();
        Loaded += async (s, e) => await LoadAllDataAsync();
    }

    private async Task LoadAllDataAsync()
    {
        await using var context = new DomDivanContext();

        // Загрузка продуктов
        _allProducts = await LoadProductsAsync();

        ApplyFiltersAndSort();
        StatusTextBlock.Text = $"Загружено товаров: {_allProducts.Count}";
        LowQuantityCount.Text = $"{_allProducts.Sum(p => p.NotificationCount)}";
    }

    private async Task<List<ProductViewAdmin>> LoadProductsAsync()
    {
        var productsWithDetails = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Cloth)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Photos)
            .Select(p => new
            {
                Product = p,
                Sofa = _context.Sofas
                    .Include(s => s.Filler)
                    .Include(s => s.FoldingMechanism)
                    .FirstOrDefault(s => s.ProductId == p.Id),
                Armchair = _context.Armchairs
                    .Include(a => a.Filler)
                    .FirstOrDefault(a => a.ProductId == p.Id)
            })
            .ToListAsync();

        return productsWithDetails.Select(x => new ProductViewAdmin
        {
            ProductId = x.Product.Id,
            Name = x.Product.Name,
            Category = x.Product.Category.Name,
            Price = x.Product.Price,
            Discount = x.Product.Discount,
            Filler = x.Sofa?.Filler?.Name ?? x.Armchair?.Filler?.Name,
            Mechanism = x.Sofa?.FoldingMechanism?.Name,
            Variants = x.Product.Variants
        }).ToList();
    }

    private void ApplyFiltersAndSort()
    {
        if (_allProducts == null) return;

        var filteredProducts = _allProducts.AsQueryable();

        // Применяем фильтры
        switch (FilterComboBox.SelectedIndex)
        {
            case 1: // Со скидкой
                filteredProducts = filteredProducts.Where(p => p.Discount > 0);
                break;
            case 2: // Мало на складе
                filteredProducts = filteredProducts.Where(p => p.Variants.Any(v => v.StockQuantity < 5));
                break;
        }

        // Применяем поиск
        if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
        {
            var searchText = SearchTextBox.Text.ToLower();
            filteredProducts = filteredProducts.Where(p => p.Name.ToLower().Contains(searchText));
        }

        // Применяем сортировку
        switch (SortComboBox.SelectedIndex)
        {
            case 0: // По названию (А-Я)
                filteredProducts = filteredProducts.OrderBy(p => p.FullName);
                break;
            case 1: // По названию (Я-А)
                filteredProducts = filteredProducts.OrderByDescending(p => p.FullName);
                break;
            case 2: // По цене (↑)
                filteredProducts = filteredProducts.OrderBy(p => p.DiscountPrice);
                break;
            case 3: // По цене (↓)
                filteredProducts = filteredProducts.OrderByDescending(p => p.DiscountPrice);
                break;
            case 4: // По скидке (↑)
                filteredProducts = filteredProducts.OrderBy(p => p.Discount ?? 0);
                break;
            case 5: // По скидке (↓)
                filteredProducts = filteredProducts.OrderByDescending(p => p.Discount ?? 0);
                break;
        }

        ProductsListView.ItemsSource = filteredProducts.ToList();
    }

    private async void AddProduct_Click(object sender, RoutedEventArgs e)
    {
        var editorWindow = new ProductEditorWindow();
        if (editorWindow.ShowDialog() == true)
        {
            _allProducts = await LoadProductsAsync(); // Перезагружаем список после добавления
        }
    }

    private void ManageParameters_Click(object sender, RoutedEventArgs e)
    {
        new ParametersWindow().ShowDialog();
    }

    private async void ProductsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ProductsListView.SelectedItem is ProductViewAdmin selectedProduct)
        {
            Product product = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Cloth)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Photos)
                .FirstOrDefault(p => p.Id == selectedProduct.ProductId);

            string imageDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                                "..", "..", "..", "Image");
            imageDirPath = Path.GetFullPath(imageDirPath);

            foreach (var variant in product.Variants)
            {
                variant.LastBuyPrice = _context.ProductsInSupply
                                                .AsNoTracking()
                                                .Include(p => p.Supply)
                                                .OrderByDescending(p => p.Supply.SupplyDate)
                                                .Where(v => v.VariantId == variant.Id)
                                                .Select(p => (decimal?)p.Price)
                                                .FirstOrDefault();

                foreach (var photo in variant.Photos)
                {
                    photo.PhotoPath = $"{imageDirPath}\\{photo.PhotoName}";
                }
            }

            var editorWindow = new ProductEditorWindow(product);
            if (editorWindow.ShowDialog() == true)
            {
                _allProducts = await LoadProductsAsync();
                ApplyFiltersAndSort();
            }
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFiltersAndSort();
    }

    private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersAndSort();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplyFiltersAndSort();
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

    private void GridIcon_MouseDown(object sender, MouseButtonEventArgs e)
    {
        new LowStockNotificationWindow().Show();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
