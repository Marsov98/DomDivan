using DomDivan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для ProductViewWindow.xaml
/// </summary>
public partial class ProductViewWindow : Window
{
    private ProductViewModel _viewModel;

    public ProductViewWindow(Product product)
    {
        InitializeComponent();
        _viewModel = new ProductViewModel(product);
        DataContext = _viewModel;

        // Загружаем первое изображение
        if (_viewModel.CurrentVariant.Photos.Any())
        {
            LoadImage(_viewModel.CurrentVariant.Photos.First().PhotoName);
        }
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

    private void Thumbnail_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is Border border && border.DataContext is PhotoProduct photo)
        {
            LoadImage(photo.PhotoName);
        }
    }

    private void ColorVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<Models.Color> option)
        {
            _viewModel.SelectColor(option.Color.Id);
            UpdateMainImage();
        }
    }

    private void ClothVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<Cloth> option)
        {
            _viewModel.SelectCloth(option.Cloth.Id);
            UpdateMainImage();
        }
    }

    private void SofaTypeVariation_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.DataContext is VariantOption<SofaType> option)
        {
            _viewModel.SelectSofaType(option.SofaType.Id);
            UpdateMainImage();
        }
    }

    private void UpdateMainImage()
    {
        if (_viewModel.CurrentVariant.Photos.Any())
        {
            var primaryPhoto = _viewModel.CurrentVariant.Photos.FirstOrDefault(p => p.IsPrimary) ??
                               _viewModel.CurrentVariant.Photos.First();
            LoadImage(primaryPhoto.PhotoName);
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Здесь должна быть логика возврата на предыдущее окно
        this.Close();
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Товар добавлен в корзину", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

public class ProductViewModel
{
    public Product Product { get; }
    public Variant CurrentVariant { get; private set; }

    public List<VariantOption<Models.Color>> ColorOptions { get; } = new();
    public List<VariantOption<Cloth>> ClothOptions { get; } = new();
    public List<VariantOption<SofaType>> SofaTypeOptions { get; } = new();

    public bool HasColors => ColorOptions.Any();
    public bool HasCloths => ClothOptions.Any();
    public bool HasSofaTypes => SofaTypeOptions.Any();

    public bool IsSofa => Product is Sofa;
    public bool IsBed => Product is Bed;
    public bool IsArmchair => Product is Armchair;

    public Sofa SofaInfo => Product as Sofa;
    public Bed BedInfo => Product as Bed;
    public Armchair ArmchairInfo => Product as Armchair;

    public ProductViewModel(Product product)
    {
        Product = product ?? throw new ArgumentNullException(nameof(product));
        CurrentVariant = product.Variants.FirstOrDefault();

        InitializeVariants();
    }

    private void InitializeVariants()
    {
        // Группируем варианты по цветам
        var colorGroups = Product.Variants
            .GroupBy(v => v.Color)
            .Select(g => new VariantOption<Models.Color>
            {
                Color = g.Key,
                IsSelected = g.Key.Id == CurrentVariant.ColorId
            });

        ColorOptions.AddRange(colorGroups);

        // Группируем варианты по тканям
        var clothGroups = Product.Variants
            .GroupBy(v => v.Cloth)
            .Select(g => new VariantOption<Cloth>
            {
                Cloth = g.Key,
                IsSelected = g.Key.Id == CurrentVariant.ClothId
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
                    IsSelected = g.Key.Id == CurrentVariant.SofaTypeId
                });

            SofaTypeOptions.AddRange(sofaTypeGroups);
        }
    }

    public void SelectColor(int colorId)
    {
        var variant = Product.Variants
            .FirstOrDefault(v => v.ColorId == colorId &&
                               v.ClothId == CurrentVariant.ClothId &&
                               (v.SofaTypeId == CurrentVariant.SofaTypeId ||
                                (v.SofaTypeId == null && CurrentVariant.SofaTypeId == null)));

        if (variant != null)
        {
            CurrentVariant = variant;
        }
    }

    public void SelectCloth(int clothId)
    {
        var variant = Product.Variants
            .FirstOrDefault(v => v.ClothId == clothId &&
                               v.ColorId == CurrentVariant.ColorId &&
                               (v.SofaTypeId == CurrentVariant.SofaTypeId ||
                                (v.SofaTypeId == null && CurrentVariant.SofaTypeId == null)));

        if (variant != null)
        {
            CurrentVariant = variant;
        }
    }

    public void SelectSofaType(int? sofaTypeId)
    {
        var variant = Product.Variants
            .FirstOrDefault(v => v.SofaTypeId == sofaTypeId &&
                               v.ColorId == CurrentVariant.ColorId &&
                               v.ClothId == CurrentVariant.ClothId);

        if (variant != null)
        {
            CurrentVariant = variant;
        }
    }
}

public class VariantOption<T>
{
    public T Color { get; set; }
    public T Cloth { get; set; }
    public T SofaType { get; set; }
    public bool IsSelected { get; set; }
}