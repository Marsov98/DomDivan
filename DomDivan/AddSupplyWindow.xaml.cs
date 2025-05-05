using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для AddSupplyWindow.xaml
/// </summary>
public partial class AddSupplyWindow : Window
{
    private readonly DomDivanContext _context;
    private ObservableCollection<ProductInSupply> _productsInSupply = new ObservableCollection<ProductInSupply>();
    private List<Category> _categories = new List<Category>();
    private List<SupplyProductInfo> _productsInfo = new List<SupplyProductInfo>();
    private List<Supplier> _suppliers = new List<Supplier>();

    public AddSupplyWindow()
    {
        InitializeComponent();
        _context = new DomDivanContext();
        LoadData();
        ProductsDataGrid.ItemsSource = _productsInSupply;
    }

    private void LoadData()
    {
        // Загрузка поставщиков
        _suppliers = _context.Suppliers.ToList();
        SupplierComboBox.ItemsSource = _suppliers;
        SupplierComboBox.DisplayMemberPath = "CompanyName";
        SupplierComboBox.SelectedValuePath = "Id";

        // Загрузка товаров и их вариаций
        _productsInfo = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Color)
            .Include(p => p.Variants)
                .ThenInclude(v => v.Cloth)
            .Include(p => p.Variants)
                .ThenInclude(v => v.SofaType)
            .Select(p => new SupplyProductInfo
            {
                ProductId = p.Id,
                ProductCategory = p.Category.Name,
                ProductName = p.Name,
                SupplyVariants = p.Variants.Select(v => new SupplyVariantInfo
                {
                    VariantId = v.Id,
                    VariantTitle = v.SofaType == null ? 
                                $"{v.Color.Name} {v.Cloth.Name}" : 
                                $"{v.Color.Name}, {v.Cloth.Name}, {v.SofaType.Name}"
                }).ToList()
            })
            .ToList();

        ProductComboBox.ItemsSource = _productsInfo;
        ProductComboBox.DisplayMemberPath = "ProductTitle";
        ProductComboBox.SelectedValuePath = "ProductId";
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is Category selectedCategory)
        {
            ProductComboBox.ItemsSource = _productsInfo.Where(p => p.ProductCategory == selectedCategory.Name);
            ProductComboBox.SelectedIndex = -1;
            VariantComboBox.ItemsSource = null;
        }
    }

    private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProductComboBox.SelectedItem is SupplyProductInfo selectedProduct)
        {
            VariantComboBox.ItemsSource = selectedProduct.SupplyVariants;
            VariantComboBox.DisplayMemberPath = "VariantTitle";
            VariantComboBox.SelectedValuePath = "VariantId";

            if (selectedProduct.SupplyVariants.Any())
            {
                VariantComboBox.SelectedIndex = 0;
            }
        }
    }

    private void AddProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (VariantComboBox.SelectedItem == null ||
            !int.TryParse(QuantityTextBox.Text, out int quantity) ||
            quantity <= 0 ||
            !decimal.TryParse(PriceTextBox.Text, out decimal price) ||
            price <= 0)
        {
            MessageBox.Show("Пожалуйста, заполните все поля корректными значениями");
            return;
        }

        var variantInfo = (SupplyVariantInfo)VariantComboBox.SelectedItem;
        var productInfo = (SupplyProductInfo)ProductComboBox.SelectedItem;

        // Проверяем, не добавлен ли уже этот вариант товара
        var existingItem = _productsInSupply.FirstOrDefault(p => p.VariantId == variantInfo.VariantId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            ProductsDataGrid.Items.Refresh();
        }
        else
        {
            _productsInSupply.Add(new ProductInSupply
            {
                VariantId = variantInfo.VariantId,
                Quantity = quantity,
                Price = price,
                Variant = new Variant
                {
                    Id = variantInfo.VariantId,
                    Product = new Product
                    {
                        Id = productInfo.ProductId,
                        Name = productInfo.ProductTitle
                    }
                }
            });
        }
    }

    private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is ProductInSupply product)
        {
            _productsInSupply.Remove(product);
        }
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (SupplierComboBox.SelectedItem == null)
        {
            MessageBox.Show("Пожалуйста, выберите поставщика");
            return;
        }

        if (_productsInSupply.Count == 0)
        {
            MessageBox.Show("Добавьте хотя бы один товар в поставку");
            return;
        }

        try
        {
            var supply = new Supply
            {
                SupplierId = (int)SupplierComboBox.SelectedValue,
                SupplyDate = DateTime.Now,
                ProductInSupply = _productsInSupply.ToList()
            };

            _context.Supplies.Add(supply);
            _context.SaveChanges();

            MessageBox.Show("Поставка успешно сохранена");
            this.DialogResult = true;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при сохранении поставки: {ex.Message}");
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        new SuppliesViewWindow().Show();
        this.Close();
    }

    private void AddNewProductButton_Click(object sender, RoutedEventArgs e)
    {
        var productEditor = new ProductEditorWindow();
        if (productEditor.ShowDialog() == true)
        {
            // Перезагружаем список товаров после добавления нового
            LoadData();
        }
    }

    private void AddNewSupplierButton_Click(object sender, RoutedEventArgs e)
    {
        var supplierEditor = new EditSupplierWindow();
        if (supplierEditor.ShowDialog() == true)
        {
            // Перезагружаем список поставщиков после добавления нового
            LoadData();
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}