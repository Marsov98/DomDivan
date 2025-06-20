﻿using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DomDivan;

public partial class CatalogWindow : Window, INotifyPropertyChanged
{
    #region ViewModel Properties

    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); ApplyFilters(); }
    }

    private ObservableCollection<ProductViewUser> _filteredProducts = new();
    public ObservableCollection<ProductViewUser> FilteredProducts
    {
        get => _filteredProducts;
        set { _filteredProducts = value; OnPropertyChanged(); }
    }

    private List<ProductViewUser> _allProducts = new();

    // Сортировка
    public ObservableCollection<SortOption> SortOptions { get; } = new()
    {
        new SortOption("Без сортировки", SortDirection.None),
        new SortOption("По возрастанию цены", SortDirection.Ascending),
        new SortOption("По убыванию цены", SortDirection.Descending)
    };

    private SortOption _selectedSortOption;
    public SortOption SelectedSortOption
    {
        get => _selectedSortOption;
        set { _selectedSortOption = value; OnPropertyChanged(); ApplyFilters(); }
    }

    // Фильтры категорий
    private bool _filterBed = true;
    public bool FilterBed
    {
        get => _filterBed;
        set { _filterBed = value; OnPropertyChanged(); ApplyFilters(); }
    }

    private bool _filterSofa = true;
    public bool FilterSofa
    {
        get => _filterSofa;
        set { _filterSofa = value; OnPropertyChanged(); ApplyFilters(); }
    }

    private bool _filterArmchair = true;
    public bool FilterArmchair
    {
        get => _filterArmchair;
        set { _filterArmchair = value; OnPropertyChanged(); ApplyFilters(); }
    }

    // Фильтры характеристик
    public ObservableCollection<FilterItem> Colors { get; } = new();
    public ObservableCollection<FilterItem> Cloths { get; } = new();
    public ObservableCollection<FilterItem> Fillings { get; } = new();
    public ObservableCollection<FilterItem> Mechanisms { get; } = new();
    public ObservableCollection<FilterItem> SofaTypes { get; } = new();

    // Цена
    private int _minPrice;
    public int MinPrice
    {
        get => _minPrice;
        set { _minPrice = value; OnPropertyChanged(); ApplyFilters(); }
    }

    private int _maxPrice;
    public int MaxPrice
    {
        get => _maxPrice;
        set { _maxPrice = value; OnPropertyChanged(); ApplyFilters(); }
    }

    private int _minViewPrice;
    public int MinViewPrice
    {
        get => _minViewPrice;
        set { _minViewPrice = value; OnPropertyChanged(); }
    }

    private int _maxViewPrice;
    public int MaxViewPrice
    {
        get => _maxViewPrice;
        set { _maxViewPrice = value; OnPropertyChanged(); }
    }

    // UI
    private int _columnCount = 2;
    public int ColumnCount
    {
        get => _columnCount;
        set { _columnCount = value; OnPropertyChanged(); }
    }

    // Директория папки фотографий
    public string imageDirPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Image"));

    #endregion

    public CatalogWindow()
    {
        InitializeComponent();
        DataContext = this;

        Loaded += async (s, e) => await InitializeAsync();
        searchBox.TextChanged += (s, e) => SearchText = searchBox.Text;
    }

    #region Initialization

    private async Task InitializeAsync()
    {
        try
        {
            await LoadAllDataAsync();
            InitializePriceRange();
            SelectedSortOption = SortOptions.First();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadAllDataAsync()
    {
        await using var context = new DomDivanContext();

        // Загрузка продуктов
        _allProducts = await LoadProductsAsync(context);
        FilteredProducts = new ObservableCollection<ProductViewUser>(_allProducts);

        // Загрузка фильтров
        await LoadFiltersAsync(context);
    }

    private async Task<List<ProductViewUser>> LoadProductsAsync(DomDivanContext context)
    {


        var variantsWithDetails = await context.Variants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p.Category)
            .Include(v => v.Color)
            .Include(v => v.Cloth)
            .Include(v => v.SofaType)
            .Include(v => v.Photos)
            .Select(v => new
            {
                Variant = v,
                Sofa = context.Sofas
                    .Include(s => s.Filler)
                    .Include(s => s.FoldingMechanism)
                    .FirstOrDefault(s => s.ProductId == v.ProductId),
                Armchair = context.Armchairs
                    .Include(a => a.Filler)
                    .FirstOrDefault(a => a.ProductId == v.ProductId)
            })
            .Where(x => x.Variant.Photos.Any(p => p.IsPrimary))
            .ToListAsync();

        return variantsWithDetails.Select(x => new ProductViewUser
        {
            VariantId = x.Variant.Id,
            ProductId = x.Variant.ProductId,
            Name = x.Variant.Product.Name,
            Category = x.Variant.Product.Category.Name,
            Price = x.Variant.Product.Price,
            Discount = x.Variant.Product.Discount,
            Color = x.Variant.Color.Name,
            Cloth = x.Variant.Cloth.Name,
            SofaType = x.Variant.SofaType?.Name,
            Filler = x.Sofa?.Filler?.Name ?? x.Armchair?.Filler?.Name,
            Mechanism = x.Sofa?.FoldingMechanism?.Name,
            Photo = $"{imageDirPath}\\{x.Variant.Photos.First(p => p.IsPrimary).PhotoName}",
            StockQuantity = x.Variant.StockQuantity,
            OutOfStock = x.Variant.StockQuantity == 0,
            IsInCart = false
        }).ToList();
    }

    private async Task LoadFiltersAsync(DomDivanContext context)
    {
        // Загрузка цветов
        Colors.Clear();
        var colors = await context.Colors
            .AsNoTracking()
            .Select(c => c.Name)
            .ToListAsync();
        foreach (var color in colors)
        {
            var item = new FilterItem(color);
            item.SelectionChanged += OnFilterSelectionChanged;
            Colors.Add(item);
        }

        // Загрузка тканей
        Cloths.Clear();
        var cloths = await context.Cloths
            .AsNoTracking()
            .Select(c => c.Name)
            .ToListAsync();
        foreach (var cloth in cloths)
        {
            var item = new FilterItem(cloth);
            item.SelectionChanged += OnFilterSelectionChanged;
            Cloths.Add(item);
        }

        // Загрузка наполнителей
        Fillings.Clear();
        var fillings = await context.Fillers
            .AsNoTracking()
            .Select(f => f.Name)
            .ToListAsync();
        foreach (var filling in fillings)
        {
            var item = new FilterItem(filling);
            item.SelectionChanged += OnFilterSelectionChanged;
            Fillings.Add(item);
        }

        // Загрузка механизмов
        Mechanisms.Clear();
        var mechanisms = await context.FoldingMechanisms
            .AsNoTracking()
            .Select(m => m.Name)
            .ToListAsync();
        foreach (var mechanism in mechanisms)
        {
            var item = new FilterItem(mechanism);
            item.SelectionChanged += OnFilterSelectionChanged;
            Mechanisms.Add(item);
        }

        // Загрузка типо диванов
        SofaTypes.Clear();
        var sofaTypes = await context.SofaTypes
            .AsNoTracking()
            .Select(m => m.Name)
            .ToListAsync();
        foreach (var sofaType in sofaTypes)
        {
            var item = new FilterItem(sofaType);
            item.SelectionChanged += OnFilterSelectionChanged;
            SofaTypes.Add(item);
        }
    }

    private void InitializePriceRange()
    {
        if (!_allProducts.Any()) return;

        MinViewPrice = (int)_allProducts.Min(p => p.Price);
        MaxViewPrice = (int)_allProducts.Max(p => p.Price);
        MinPrice = MinViewPrice;
        MaxPrice = MaxViewPrice;

        OnPropertyChanged(nameof(MinViewPrice));
        OnPropertyChanged(nameof(MaxViewPrice));
    }

    #endregion

    #region Filtering

    private void ApplyFilters()
    {
        try
        {
            UpdateFiltersVisibility();

            if (_allProducts == null || !_allProducts.Any())
            {
                FilteredProducts.Clear();
                return;
            }

            // Создаем копию для фильтрации
            var filtered = new List<ProductViewUser>(_allProducts);

            // Фильтрация по категориям
            filtered = filtered.Where(p =>
                (FilterBed && p.Category == "Кровать") ||
                (FilterSofa && p.Category == "Диван") ||
                (FilterArmchair && p.Category == "Кресло"))
                .ToList();

            // Фильтрация по цене
            filtered = filtered.Where(p => p.DiscountPrice >= MinPrice && p.DiscountPrice <= MaxPrice).ToList();

            // Фильтрация по поиску
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p =>
                    p.Name.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (!string.IsNullOrEmpty(p.Description) &&
                     p.Description.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) >= 0))
                    .ToList();
            }

            // Фильтрация по цвету
            if (Colors.Any(c => c.IsSelected))
            {
                var selectedColors = new HashSet<string>(Colors.Where(c => c.IsSelected).Select(c => c.Name));
                filtered = filtered.Where(p => selectedColors.Contains(p.Color)).ToList();
            }

            // Фильтрация по ткани
            if (Cloths.Any(c => c.IsSelected))
            {
                var selectedCloths = new HashSet<string>(Cloths.Where(c => c.IsSelected).Select(c => c.Name));
                filtered = filtered.Where(p => selectedCloths.Contains(p.Cloth)).ToList();
            }

            // Фильтрация по наполнителю
            if (Fillings.Any(f => f.IsSelected) && FillingExpander.Visibility == Visibility.Visible)
            {
                var selectedFillings = new HashSet<string>(Fillings.Where(f => f.IsSelected).Select(f => f.Name));
                filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Filler) &&
                                              selectedFillings.Contains(p.Filler))
                    .ToList();
            }

            // Фильтрация по механизму
            if (Mechanisms.Any(m => m.IsSelected) && MechanismExpander.Visibility == Visibility.Visible)
            {
                var selectedMechanisms = new HashSet<string>(Mechanisms.Where(m => m.IsSelected).Select(m => m.Name));
                filtered = filtered.Where(p => !string.IsNullOrEmpty(p.Mechanism) &&
                                             selectedMechanisms.Contains(p.Mechanism))
                    .ToList();
            }

            // Фильтрация по типу
            if (SofaTypes.Any(m => m.IsSelected) && SofaTypesExpander.Visibility == Visibility.Visible)
            {
                var selectedSofaTypes = new HashSet<string>(SofaTypes.Where(m => m.IsSelected).Select(m => m.Name));
                filtered = filtered.Where(p => !string.IsNullOrEmpty(p.SofaType) &&
                                             selectedSofaTypes.Contains(p.SofaType))
                    .ToList();
            }

            // Сортировка
            var sorted = SelectedSortOption?.SortDirection switch
            {
                SortDirection.Ascending => filtered.OrderBy(p => p.Price),
                SortDirection.Descending => filtered.OrderByDescending(p => p.Price),
                _ => filtered.AsEnumerable()
            };

            foreach (var item in sorted)
            {
                if (item != null)
                {
                    item.IsInCart = CartService.Instance.IsInCart(item.VariantId);
                    item.CartQuantity = CartService.Instance.GetItemQuantity(item.VariantId);
                    item.IsMaxQuantity = CartService.Instance.IsMaxQuantity(item.VariantId, item.StockQuantity);
                }
            }

            // Обновление коллекции
            ListItems.ItemsSource = new ObservableCollection<ProductViewUser>(sorted);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка в ApplyFilters: {ex.Message}");
            // Восстанавливаем все продукты при ошибке
            ListItems.ItemsSource = new ObservableCollection<ProductViewUser>(_allProducts);
        }
    }

    private void UpdateFiltersVisibility()
    {
        FillingExpander.Visibility = FilterBed
            ? Visibility.Collapsed
            : Visibility.Visible;

        MechanismExpander.Visibility = (FilterBed || FilterArmchair)
            ? Visibility.Collapsed
            : Visibility.Visible;

        SofaTypesExpander.Visibility = (FilterBed || FilterArmchair)
            ? Visibility.Collapsed
            : Visibility.Visible;
    }

    #endregion

    #region Event Handlers

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Логика обработки изменения размеров окна
        double newWidth = e.NewSize.Width;
        // Минимальная ширина карточки 250 пикселей
        double cardWidth = 310;
        // Добавляем отступы между карточками
        double spacing = 20;
        ColumnCount = Math.Max(1, (int)((newWidth - 250) / (cardWidth + spacing)));
    }

    private void OnFilterSelectionChanged(object sender, EventArgs e)
    {
        ApplyFilters();
    }

    private void ViewSelected_Click(object sender, SelectionChangedEventArgs e)
    {
        var selectedItem = (sender as ListBox).SelectedItem as ProductViewUser;

        if (selectedItem != null)
        {
            new ProductViewWindow(selectedItem.ProductId, selectedItem.VariantId).Show();
            this.Close();

        }
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = (sender as Button).DataContext as ProductViewUser;

        if (selectedItem != null)
        {
            using (var db = new DomDivanContext())
            {
                var selectedVariant = db.Variants
                    .Include(v => v.Product)
                        .ThenInclude(p => p.Category)
                    .Include(v => v.Color)
                    .Include(v => v.Cloth)
                    .Include(v => v.SofaType)
                    .Include(v => v.Photos)
                    .FirstOrDefault(v => v.Id == selectedItem.VariantId);

                if (selectedVariant != null)
                {
                    CartService.Instance.AddToCart(selectedVariant);

                    ApplyFilters();

                    CartCount.Text = CartService.Instance.ItemsCount.ToString();
                }
            }
        }
    }

    private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = (sender as Button).DataContext as ProductViewUser;
        if (selectedItem != null)
        {
            CartService.Instance.UpdateQuantity(selectedItem.VariantId, CartService.Instance.GetItemQuantity(selectedItem.VariantId) + 1);
            CartCount.Text = CartService.Instance.ItemsCount.ToString();
            ApplyFilters();
        }
    }

    private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = (sender as Button).DataContext as ProductViewUser;
        if (selectedItem != null)
        {
            int newQuantity = CartService.Instance.GetItemQuantity(selectedItem.VariantId) - 1;
            if (newQuantity <= 0)
            {
                CartService.Instance.RemoveFromCart(selectedItem.VariantId);
            }
            else
            {
                CartService.Instance.UpdateQuantity(selectedItem.VariantId, newQuantity);
            }
            CartCount.Text = CartService.Instance.ItemsCount.ToString();
            ApplyFilters();
        }
    }

    private void GridIcon_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var cartWindow = new CartWindow();
        cartWindow.Show();
        this.Close();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        new LoginWindow().Show();
        this.Close();
    }

    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

#region Helper Classes

public class SortOption
{
    public string Name { get; }
    public SortDirection SortDirection { get; }

    public SortOption(string name, SortDirection sortDirection)
    {
        Name = name;
        SortDirection = sortDirection;
    }
}

public enum SortDirection { None, Ascending, Descending }

public class FilterItem : INotifyPropertyChanged
{
    public string Name { get; }

    private bool _isSelected = true;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler SelectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public FilterItem(string name) => Name = name;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public class NullToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool invert = parameter as string == "invert";
        bool isVisible = value != null;

        if (invert) isVisible = !isVisible;

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DiscountToStrikethroughConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? TextDecorations.Strikethrough : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DiscountToGrayTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null ? Brushes.Gray : Brushes.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

#endregion
