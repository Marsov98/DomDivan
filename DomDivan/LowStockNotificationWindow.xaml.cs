using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для LowStockNotificationWindow.xaml
/// </summary>
public partial class LowStockNotificationWindow : Window
{
    public LowStockNotificationWindow()
    {
        InitializeComponent();
        LoadLowStockProducts();
    }

    private void LoadLowStockProducts()
    {
        using (var context = new DomDivanContext())
        {
            // Получаем вариации с количеством меньше 5
            var lowStockVariants = context.Variants
                .Include(v => v.Product)
                    .ThenInclude(p => p.Category)
                .Include(v => v.Color)
                .Include(v => v.Cloth)
                .Include(v => v.SofaType)
                .Where(v => v.StockQuantity < 5)
                .ToList();

            var lowStockProducts = new List<SmallQuantityProductView>();

            foreach (var variant in lowStockVariants)
            {
                // Получаем информацию о последних поставщиках
                var lastSupplies = context.ProductsInSupply
                    .Include(p => p.Supply)
                    .ThenInclude(s => s.Supplier)
                    .Where(p => p.VariantId == variant.Id)
                    .OrderByDescending(p => p.Supply.SupplyDate)
                    .Take(3) // Берем 3 последних поставки
                    .ToList();

                var product = new SmallQuantityProductView
                {
                    ProductTitle = $"{variant.Product.Category.Name} \"{variant.Product.Name}\"",
                    VariantTitle = variant.SofaType == null ?
                                        $"{variant.Color.Name} {variant.Cloth.Name}" :
                                        $"{variant.Color.Name}, {variant.Cloth.Name}, {variant.SofaType.Name}",
                    StockQuantity = variant.StockQuantity,
                    SupplierShortLastInfos = lastSupplies.Select(s => new SupplierShortLastInfo
                    {
                        CompanyName = s.Supply.Supplier.CompanyName,
                        ContactPerson = s.Supply.Supplier.ContactPerson,
                        PhoneNumber = s.Supply.Supplier.PhoneNumber,
                        LastPrice = s.Price
                    }).ToList()
                };

                lowStockProducts.Add(product);
            }

            LowStockListView.ItemsSource = lowStockProducts;
        }
    }
}