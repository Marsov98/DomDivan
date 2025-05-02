using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DomDivan;

public partial class ProductViewWindow : Window, INotifyPropertyChanged
{
    private Product _product;
    private Variant _currentVariant;
    private int _currentPhotoIndex = 0;
    private bool _isInCart;
    private bool _isVariantNotFound;
    private Models.ColorVariant _selectedColor;
    private Cloth _selectedCloth;
    private SofaType? _selectedSofaType;

    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnPropertyChanged();
        }
    }

    public Variant CurrentVariant
    {
        get => _currentVariant;
        set
        {
            _currentVariant = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CartQuantity));
            OnPropertyChanged(nameof(IsInCart));
        }
    }

    public bool IsInCart
    {
        get => _isInCart;
        set
        {
            _isInCart = value;
            OnPropertyChanged();
        }
    }

    public bool IsVariantNotFound
    {
        get => _isVariantNotFound;
        set
        {
            _isVariantNotFound = value;
            OnPropertyChanged();
        }
    }

    public Models.ColorVariant SelectedColor
    {
        get => _selectedColor;
        set
        {
            _selectedColor = value;
            OnPropertyChanged();
        }
    }

    public Cloth SelectedCloth
    {
        get => _selectedCloth;
        set
        {
            _selectedCloth = value;
            OnPropertyChanged();
        }
    }

    public SofaType? SelectedSofaType
    {
        get => _selectedSofaType;
        set
        {
            _selectedSofaType = value;
            OnPropertyChanged();
        }
    }

    public int CartQuantity { get; set; }

    public List<VariantOption<Models.ColorVariant>> ColorOptions { get; } = new();
    public List<VariantOption<Cloth>> ClothOptions { get; } = new();
    public List<VariantOption<SofaType>> SofaTypeOptions { get; } = new();

    public bool HasColors => ColorOptions.Any();
    public bool HasCloths => ClothOptions.Any();
    public bool HasSofaTypes => SofaTypeOptions.Any();

    public bool IsSofa => Product.Category.Name == "Диван";
    public bool IsBed => Product.Category.Name == "Кровать";
    public bool IsArmchair => Product.Category.Name == "Кресло";

    public Sofa SofaInfo { get; private set; }
    public Bed BedInfo { get; private set; }
    public Armchair ArmchairInfo { get; private set; }

    public ProductViewWindow(Product product, int IdVariant)
    {
        InitializeComponent();
        DataContext = this;

        LoadProductData(product, IdVariant);
    }

    private void LoadProductData(Product product, int IdVariant)
    {
        using (var db = new DomDivanContext())
        {
            switch (product.Category.Name)
            {
                case "Диван":
                    SofaInfo = db.Sofas
                        .Include(s => s.Filler)
                        .Include(s => s.FoldingMechanism)
                        .FirstOrDefault(s => s.ProductId == product.Id);
                    break;
                case "Кровать":
                    BedInfo = db.Beds.FirstOrDefault(b => b.ProductId == product.Id);
                    break;
                case "Кресло":
                    ArmchairInfo = db.Armchairs
                        .Include(a => a.Filler)
                        .FirstOrDefault(a => a.ProductId == product.Id);
                    break;
            }

            // Загружаем все связанные данные
            Product = db.Products
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Photos)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Cloth)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.SofaType)
                .Include(p => p.Category)
                .FirstOrDefault(p => p.Id == product.Id);

            CurrentVariant = Product.Variants.FirstOrDefault(v => v.Id == IdVariant);
            UpdateCartStatus();
            InitializeVariants();

            // Загружаем первое изображение
            if (CurrentVariant?.Photos?.Any() == true)
            {
                LoadImage(CurrentVariant.Photos.First().PhotoName);
                _currentPhotoIndex = 0;
            }
        }
    }

    private void InitializeVariants()
    {
        ColorOptions.Clear();
        ClothOptions.Clear();
        SofaTypeOptions.Clear();

        SelectedCloth = CurrentVariant.Cloth;
        SelectedColor = CurrentVariant.Color;
        SelectedSofaType = CurrentVariant.SofaType;

        // Группируем варианты по цветам
        var colorGroups = Product.Variants
            .GroupBy(v => v.Color)
            .Select(g => new VariantOption<Models.ColorVariant>
            {
                Color = g.Key,
                IsSelected = g.Key.Id == CurrentVariant?.ColorId
            });

        ColorOptions.AddRange(colorGroups);

        // Группируем варианты по тканям
        var clothGroups = Product.Variants
            .GroupBy(v => v.Cloth)
            .Select(g => new VariantOption<Cloth>
            {
                Cloth = g.Key,
                IsSelected = g.Key.Id == CurrentVariant?.ClothId
            });

        ClothOptions.AddRange(clothGroups);

        // Группируем варианты по типам диванов (если есть)
        if (Product.Variants.Any(v => v.SofaType != null))
        {
            var sofaTypeGroups = Product.Variants
                .Where(v => v.SofaType != null)
                .GroupBy(v => v.SofaType)
                .Select(g => new VariantOption<SofaType>
                {
                    SofaType = g.Key,
                    IsSelected = g.Key.Id == CurrentVariant?.SofaTypeId
                });

            SofaTypeOptions.AddRange(sofaTypeGroups);
        }

        OnPropertyChanged(nameof(ColorOptions));
        OnPropertyChanged(nameof(ClothOptions));
        OnPropertyChanged(nameof(SofaTypeOptions));
        OnPropertyChanged(nameof(HasColors));
        OnPropertyChanged(nameof(HasCloths));
        OnPropertyChanged(nameof(HasSofaTypes));
    }

    private void LoadImage(string imagePath)
    {
        try
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imagePath, UriKind.RelativeOrAbsolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            MainImage.Source = bitmap;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки изображения: {ex.Message}");
        }
    }

    private void Thumbnail_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is PhotoProduct photo)
        {
            LoadImage(photo.PhotoName);
            _currentPhotoIndex = CurrentVariant.Photos.IndexOf(photo);
        }
    }

    private void ColorVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<Models.ColorVariant> option)
        {
            SelectedColor = option.Color;
            SelectedVariants();
            UpdateImages();
        }
    }

    private void ClothVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<Cloth> option)
        {
            SelectedCloth = option.Cloth;
            SelectedVariants();
            UpdateImages();
        }
    }

    private void SofaTypeVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<SofaType> option)
        {
            SelectedSofaType = option.SofaType;
            SelectedVariants();
            UpdateImages();
        }
    }

    private void UpdateImages()
    {
        if (IsVariantNotFound) return;

        // Обновляем галерею миниатюр
        ThumbnailsGallery.ItemsSource = null;
        ThumbnailsGallery.ItemsSource = CurrentVariant.Photos;

        // Загружаем первое изображение новой вариации
        if (CurrentVariant.Photos.Any())
        {
            LoadImage(CurrentVariant.Photos.First().PhotoName);
            _currentPhotoIndex = 0;
        }
    }

    private void PrevPhotoButton_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVariant?.Photos?.Count == 0) return;

        _currentPhotoIndex--;
        if (_currentPhotoIndex < 0)
            _currentPhotoIndex = CurrentVariant.Photos.Count - 1;

        LoadImage(CurrentVariant.Photos[_currentPhotoIndex].PhotoName);
    }

    private void NextPhotoButton_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVariant?.Photos?.Count == 0) return;

        _currentPhotoIndex++;
        if (_currentPhotoIndex >= CurrentVariant.Photos.Count)
            _currentPhotoIndex = 0;

        LoadImage(CurrentVariant.Photos[_currentPhotoIndex].PhotoName);
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        new CatalogWindow().Show();
        this.Close();
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVariant != null)
        {
            CartService.Instance.AddToCart(CurrentVariant);
            UpdateCartStatus();
        }
    }

    private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVariant != null)
        {
            CartService.Instance.UpdateQuantity(CurrentVariant.Id,
                CartService.Instance.GetItemQuantity(CurrentVariant.Id) + 1);
            UpdateCartStatus();
        }
    }

    private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
    {
        if (CurrentVariant != null)
        {
            int newQuantity = CartService.Instance.GetItemQuantity(CurrentVariant.Id) - 1;
            if (newQuantity <= 0)
            {
                CartService.Instance.RemoveFromCart(CurrentVariant.Id);
            }
            else
            {
                CartService.Instance.UpdateQuantity(CurrentVariant.Id, newQuantity);
            }
            UpdateCartStatus();
        }
    }

    public void UpdateCartStatus()
    {
        IsInCart = CurrentVariant != null && CartService.Instance.IsInCart(CurrentVariant.Id);
        CartQuantity = CartService.Instance.GetItemQuantity(CurrentVariant?.Id ?? 0);
        OnPropertyChanged(nameof(CartQuantity));
    }

    public void SelectedVariants()
    {
        var variant = Product.Variants
            .FirstOrDefault(v => v.ColorId == SelectedColor.Id &&
                               v.ClothId == SelectedCloth.Id &&
                               (v.SofaTypeId == SelectedSofaType?.Id ||
                                (v.SofaTypeId == null && SelectedSofaType?.Id == null)));

        if (variant != null)
        {
            CurrentVariant = variant;
            IsVariantNotFound = false;
            UpdateCartStatus();
        }
        else
        {
            IsVariantNotFound = true;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}