using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DomDivan.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace DomDivan;

public partial class ProductEditorWindow : Window, INotifyPropertyChanged
{
    private readonly DomDivanContext _context;
    private Product _originalProduct;
    private bool _isEditMode;

    public event PropertyChangedEventHandler PropertyChanged;

    private const string IMAGE_FOLDER = "Images";

    public string WindowTitle => _isEditMode ? "Редактирование товара" : "Добавление товара";
    public string UniqueGroupName => Guid.NewGuid().ToString();

    public Product CurrentProduct { get; private set; }
    public Sofa Sofa { get; private set; }
    public Bed Bed { get; private set; }
    public Armchair Armchair { get; private set; }

    public ObservableCollection<Category> Categories { get; private set; }
    public ObservableCollection<Models.Color> Colors { get; private set; }
    public ObservableCollection<Cloth> Cloths { get; private set; }
    public ObservableCollection<SofaType> SofaTypes { get; private set; }
    public ObservableCollection<Filler> Fillers { get; private set; }
    public ObservableCollection<FoldingMechanism> FoldingMechanisms { get; private set; }

    public bool IsSofaCategory => _originalProduct?.Category.Name == "Диван";

    public string FormattedDiscountPrice
    {
        get
        {
            if (CurrentProduct != null)
            {
                return CurrentProduct.DiscountPrice.ToString("C");
            }
            return string.Empty;
        }
    }

    public ProductEditorWindow(Product product = null)
    {
        InitializeComponent();
        _context = new DomDivanContext();
        _originalProduct = product;
        _isEditMode = product != null;

        LoadData();
        InitializeProduct();
        UpdateDiscountPriceDisplay();
        DataContext = this;
    }

    private void LoadData()
    {
        Categories = new ObservableCollection<Category>(_context.Categories.ToList());
        Colors = new ObservableCollection<Models.Color>(_context.Colors.ToList());
        Cloths = new ObservableCollection<Cloth>(_context.Cloths.ToList());
        SofaTypes = new ObservableCollection<SofaType>(_context.SofaTypes.ToList());
        Fillers = new ObservableCollection<Filler>(_context.Fillers.ToList());
        FoldingMechanisms = new ObservableCollection<FoldingMechanism>(_context.FoldingMechanisms.ToList());
    }

    private void InitializeProduct()
    {
        if (_isEditMode)
        {
            // Клонируем продукт для редактирования
            CurrentProduct = new Product
            {
                Id = _originalProduct.Id,
                CategoryId = _originalProduct.CategoryId,
                Name = _originalProduct.Name,
                Dimensions = _originalProduct.Dimensions,
                Description = _originalProduct.Description,
                Price = _originalProduct.Price,
                Discount = _originalProduct.Discount,
                Variants = _originalProduct.Variants.Select(v => new Variant
                {
                    Id = v.Id,
                    ColorId = v.ColorId,
                    ClothId = v.ClothId,
                    SofaTypeId = v.SofaTypeId,
                    StockQuantity = v.StockQuantity,
                    Photos = v.Photos.Select(p => new PhotoProduct
                    {
                        Id = p.Id,
                        PhotoName = p.PhotoName,
                        IsPrimary = p.IsPrimary
                    }).ToList()
                }).ToList()
            };

            // Загружаем специфичные данные в зависимости от категории
            LoadCategorySpecificData();
        }
        else
        {
            CurrentProduct = new Product
            {
                Variants = new List<Variant> { new Variant { Photos = new List<PhotoProduct>() } }
            };
        }
    }

    private void LoadCategorySpecificData()
    {
        switch (CurrentProduct.CategoryId)
        {
            case 1: // Диван
                var sofa = _context.Sofas.FirstOrDefault(s => s.ProductId == CurrentProduct.Id);
                if (sofa != null)
                {
                    Sofa = new Sofa
                    {
                        Id = sofa.Id,
                        ProductId = sofa.ProductId,
                        SleepingDimensions = sofa.SleepingDimensions,
                        FillerId = sofa.FillerId,
                        FoldingMechanismId = sofa.FoldingMechanismId,
                        HasLinenDrawer = sofa.HasLinenDrawer,
                        HasArmrests = sofa.HasArmrests
                    };
                }
                break;

            case 2: // Кресло
                var armchair = _context.Armchairs.FirstOrDefault(a => a.ProductId == CurrentProduct.Id);
                if (armchair != null)
                {
                    Armchair = new Armchair
                    {
                        Id = armchair.Id,
                        ProductId = armchair.ProductId,
                        FillerId = armchair.FillerId,
                        HasLinenDrawer = armchair.HasLinenDrawer,
                        HasArmrests = armchair.HasArmrests
                    };
                }
                break;

            case 3: // Кровать
                var bed = _context.Beds.FirstOrDefault(b => b.ProductId == CurrentProduct.Id);
                if (bed != null)
                {
                    Bed = new Bed
                    {
                        Id = bed.Id,
                        ProductId = bed.ProductId,
                        SleepingDimensions = bed.SleepingDimensions
                    };
                }
                break;
        }

        UpdateCategorySpecificFields();
    }

    private void UpdateCategorySpecificFields()
    {
        CategorySpecificFieldsPanel.Children.Clear();

        if (CurrentProduct.CategoryId == 0) return;

        switch (CurrentProduct.CategoryId)
        {
            case 1: // Диван
                AddTextField("Размеры спального места:", Sofa?.SleepingDimensions ?? "", "Sofa.SleepingDimensions");
                AddComboBoxField("Наполнитель:", Fillers, Sofa?.FillerId ?? 0, "Sofa.FillerId");
                AddComboBoxField("Механизм трансформации:", FoldingMechanisms, Sofa?.FoldingMechanismId ?? 0, "Sofa.FoldingMechanismId");
                AddCheckBoxField("С ящиком для белья:", Sofa?.HasLinenDrawer ?? false, "Sofa.HasLinenDrawer");
                AddCheckBoxField("С подлокотниками:", Sofa?.HasArmrests ?? false, "Sofa.HasArmrests");
                break;

            case 2: // Кресло
                AddComboBoxField("Наполнитель:", Fillers, Armchair?.FillerId ?? 0, "Armchair.FillerId");
                AddCheckBoxField("С ящиком для белья:", Armchair?.HasLinenDrawer ?? false, "Armchair.HasLinenDrawer");
                AddCheckBoxField("С подлокотниками:", Armchair?.HasArmrests ?? false, "Armchair.HasArmrests");
                break;

            case 3: // Кровать
                AddTextField("Размеры спального места:", Bed?.SleepingDimensions ?? "", "Bed.SleepingDimensions");
                break;
        }
    }

    private void AddTextField(string label, string value, string bindingPath)
    {
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(160) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var lbl = new Label
        {
            Content = label,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0)
        };
        Grid.SetColumn(lbl, 0);

        var textBox = new TextBox
        {
            Text = value,
            Tag = bindingPath,
            VerticalAlignment = VerticalAlignment.Center
        };
        textBox.TextChanged += CategoryField_TextChanged;
        Grid.SetColumn(textBox, 1);

        grid.Children.Add(lbl);
        grid.Children.Add(textBox);

        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
        stackPanel.Children.Add(grid);
        CategorySpecificFieldsPanel.Children.Add(stackPanel);
    }

    private void AddComboBoxField(string label, IEnumerable<object> items, int selectedId, string bindingPath)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };
        stackPanel.Children.Add(new Label { Content = label, Width = 160, VerticalAlignment = VerticalAlignment.Center });

        var comboBox = new ComboBox
        {
            DisplayMemberPath = "Name",
            SelectedValuePath = "Id",
            SelectedValue = selectedId,
            Width = 200,
            Tag = bindingPath,
            ItemsSource = items
        };
        comboBox.SelectionChanged += CategoryField_SelectionChanged;
        stackPanel.Children.Add(comboBox);

        CategorySpecificFieldsPanel.Children.Add(stackPanel);
    }

    private void AddCheckBoxField(string label, bool isChecked, string bindingPath)
    {
        var stackPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 5, 0, 5) };

        var checkBox = new CheckBox
        {
            Content = label,
            IsChecked = isChecked,
            Tag = bindingPath,
            VerticalContentAlignment = VerticalAlignment.Center
        };
        checkBox.Checked += CategoryField_Checked;
        checkBox.Unchecked += CategoryField_Checked;
        stackPanel.Children.Add(checkBox);

        CategorySpecificFieldsPanel.Children.Add(stackPanel);
    }

    private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CategoryComboBox.SelectedItem is Category selectedCategory)
        {
            CurrentProduct.CategoryId = selectedCategory.Id;
            OnPropertyChanged(nameof(IsSofaCategory));
            UpdateCategorySpecificFields();
        }
    }

    private void UpdateDiscountPriceDisplay()
    {
        OnPropertyChanged(nameof(FormattedDiscountPrice));
    }

    private void DiscountPrice_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateDiscountPriceDisplay();
    }

    private void CategoryField_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.Tag is string propertyPath)
        {
            UpdateCategorySpecificProperty(propertyPath, textBox.Text);
        }
    }

    private void CategoryField_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.Tag is string propertyPath)
        {
            UpdateCategorySpecificProperty(propertyPath, comboBox.SelectedValue);
        }
    }

    private void CategoryField_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox checkBox && checkBox.Tag is string propertyPath)
        {
            UpdateCategorySpecificProperty(propertyPath, checkBox.IsChecked);
        }
    }

    private void UpdateCategorySpecificProperty(string propertyPath, object value)
    {
        try
        {
            var parts = propertyPath.Split('.');
            object obj = null;

            switch (parts[0])
            {
                case "Sofa":
                    obj = Sofa ?? (Sofa = new Sofa { ProductId = CurrentProduct.Id });
                    break;
                case "Armchair":
                    obj = Armchair ?? (Armchair = new Armchair { ProductId = CurrentProduct.Id });
                    break;
                case "Bed":
                    obj = Bed ?? (Bed = new Bed { ProductId = CurrentProduct.Id });
                    break;
            }

            if (obj == null) return;

            var prop = obj.GetType().GetProperty(parts[1]);
            prop?.SetValue(obj, value);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при обновлении поля: {ex.Message}");
        }
    }

    private void AddVariant_Click(object sender, RoutedEventArgs e)
    {
        CurrentProduct.Variants.Add(new Variant
        {
            Photos = new List<PhotoProduct>(),
            StockQuantity = 0
        });

        VariantsItemsControl.Items.Refresh();
    }

    private void RemoveVariant_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Variant variant)
        {
            CurrentProduct.Variants.Remove(variant);
            VariantsItemsControl.Items.Refresh();
        }

    }

    private void AddPhoto_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is Variant variant)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Multiselect = true
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (var fileName in openFileDialog.FileNames)
                {
                    // Создаем новую фотографию
                    var newPhoto = new PhotoProduct
                    {
                        PhotoName = fileName,
                        IsPrimary = variant.Photos.Count == 0 // Первая фото становится основной
                    };

                    // Добавляем в коллекцию
                    variant.Photos.Add(newPhoto);

                    // Если это первая фотография, делаем ее основной
                    if (variant.Photos.Count == 1)
                    {
                        newPhoto.IsPrimary = true;
                    }
                }

                // Обновляем привязку данных
                VariantsItemsControl.Items.Refresh();
            }
        }
    }

    private void RemovePhoto_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is PhotoProduct photo)
        {
            // Получаем ItemsControl, содержащий фотографии
            if (FindVisualParent<ItemsControl>(button)?.DataContext is Variant variant)
            {
                // Удаляем фотографию
                variant.Photos.Remove(photo);

                // Если удалили основное фото и есть другие фото
                if (photo.IsPrimary && variant.Photos.Count > 0)
                {
                    variant.Photos[0].IsPrimary = true;
                }

                // Обновляем привязку данных
                VariantsItemsControl.Items.Refresh();
            }
        }
    }

    private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        var parentObject = VisualTreeHelper.GetParent(child);
        if (parentObject == null) return null;
        if (parentObject is T parent) return parent;
        return FindVisualParent<T>(parentObject);
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) return;

        try
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                // 1. Сохраняем основной продукт
                if (_isEditMode)
                {
                    _context.Entry(_originalProduct).CurrentValues.SetValues(CurrentProduct);
                    _context.Entry(_originalProduct).State = EntityState.Modified;
                }
                else
                {
                    _context.Products.Add(CurrentProduct);
                }
                _context.SaveChanges();

                // 2. Сохраняем специфичные данные (прямо здесь, без вызова метода)
                switch (CurrentProduct.CategoryId)
                {
                    case 1 when Sofa != null:
                        Sofa.ProductId = CurrentProduct.Id;
                        if (_context.Sofas.Any(s => s.ProductId == CurrentProduct.Id))
                        {
                            var existing = _context.Sofas.First(s => s.ProductId == CurrentProduct.Id);
                            _context.Entry(existing).CurrentValues.SetValues(Sofa);
                            _context.Entry(existing).State = EntityState.Modified;
                        }
                        else
                        {
                            _context.Sofas.Add(Sofa);
                        }
                        break;
                    case 2 when Armchair != null:
                        Armchair.ProductId = CurrentProduct.Id;
                        if (_context.Armchairs.Any(s => s.ProductId == CurrentProduct.Id))
                        {
                            var existing = _context.Armchairs.First(s => s.ProductId == CurrentProduct.Id);
                            _context.Entry(existing).CurrentValues.SetValues(Armchair);
                            _context.Entry(existing).State = EntityState.Modified;
                        }
                        else
                        {
                            _context.Armchairs.Add(Armchair);
                        }
                        break;
                    case 3 when Bed != null:
                        Bed.ProductId = CurrentProduct.Id;
                        if (_context.Beds.Any(s => s.ProductId == CurrentProduct.Id))
                        {
                            var existing = _context.Beds.First(s => s.ProductId == CurrentProduct.Id);
                            _context.Entry(existing).CurrentValues.SetValues(Bed);
                            _context.Entry(existing).State = EntityState.Modified;
                        }
                        else
                        {
                            _context.Beds.Add(Bed);
                        }
                        break;
                }
                _context.SaveChanges();

                // 3. Сохраняем вариации (прямо здесь)
                // Удаляем старые вариации, которых нет в CurrentProduct.Variants
                var existingVariantIds = CurrentProduct.Variants.Select(v => v.Id).ToList();
                var variantsToRemove = _context.Variants
                    .Where(v => v.ProductId == CurrentProduct.Id && !existingVariantIds.Contains(v.Id))
                    .ToList();

                _context.Variants.RemoveRange(variantsToRemove);
                _context.SaveChanges();  // Важно сохранить перед добавлением новых

                foreach (var variant in CurrentProduct.Variants)
                {
                    variant.ProductId = CurrentProduct.Id;  // Всегда привязываем к продукту

                    if (variant.Id == 0)
                    {
                        // Новая вариация (Id = 0)
                        _context.Variants.Add(variant);
                    }
                    else
                    {
                        // Обновляем существующую вариацию
                        var existingVariant = _context.Variants
                            .Include(v => v.Photos)  // Важно: загружаем связанные фото
                            .FirstOrDefault(v => v.Id == variant.Id);

                        if (existingVariant != null)
                        {
                            // Копируем свойства, кроме фото
                            _context.Entry(existingVariant).CurrentValues.SetValues(variant);
                            _context.Entry(existingVariant).State = EntityState.Modified;
                        }
                    }

                    _context.SaveChanges();  // Сохраняем, чтобы получить Id вариации

                    // Обработка фотографий
                    var existingPhotoIds = variant.Photos
                        .Where(p => p.Id != 0)
                        .Select(p => p.Id)
                        .ToList();

                    // Удаляем фото, которых нет в текущем списке
                    var photosToRemove = _context.Photos
                        .Where(p => p.VariantId == variant.Id && !existingPhotoIds.Contains(p.Id))
                        .ToList();

                    _context.Photos.RemoveRange(photosToRemove);

                    foreach (var photo in variant.Photos)
                    {
                        photo.VariantId = variant.Id;  // Привязываем к вариации

                        if (photo.Id == 0)
                        {
                            // Новая фотография
                            _context.Photos.Add(photo);
                        }
                        else
                        {
                            // Обновляем существующую фотографию
                            var existingPhoto = _context.Photos.FirstOrDefault(p => p.Id == photo.Id);
                            if (existingPhoto != null)
                            {
                                _context.Entry(existingPhoto).CurrentValues.SetValues(photo);
                                _context.Entry(existingPhoto).State = EntityState.Modified;
                            }
                        }
                    }
                    _context.SaveChanges();  // Сохраняем изменения фото
                }

                transaction.Commit();
                DialogResult = true;
                Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}\n{ex.InnerException?.Message}");
        }
    }

    private bool ValidateInput()
    {
        if (CurrentProduct.CategoryId == 0)
        {
            MessageBox.Show("Выберите категорию товара");
            return false;
        }

        if (string.IsNullOrWhiteSpace(CurrentProduct.Name))
        {
            MessageBox.Show("Введите название товара");
            return false;
        }

        if (CurrentProduct.Price <= 0)
        {
            MessageBox.Show("Цена должна быть больше нуля");
            return false;
        }

        if (CurrentProduct.Discount.HasValue && (CurrentProduct.Discount < 0 || CurrentProduct.Discount > 100))
        {
            MessageBox.Show("Скидка должна быть в диапазоне от 0 до 100%");
            return false;
        }

        if (!CurrentProduct.Variants.Any())
        {
            MessageBox.Show("Добавьте хотя бы одну вариацию товара");
            return false;
        }

        foreach (var variant in CurrentProduct.Variants)
        {
            if (variant.ColorId == 0)
            {
                MessageBox.Show("Выберите цвет для всех вариаций");
                return false;
            }

            if (variant.ClothId == 0)
            {
                MessageBox.Show("Выберите ткань для всех вариаций");
                return false;
            }

            if (IsSofaCategory && variant.SofaTypeId == 0)
            {
                MessageBox.Show("Выберите тип дивана для всех вариаций");
                return false;
            }

            if (variant.StockQuantity < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным");
                return false;
            }

            if (!variant.Photos.Any())
            {
                MessageBox.Show("Добавьте хотя бы одно фото для каждой вариации");
                return false;
            }

            if (variant.Photos.Count(p => p.IsPrimary) != 1)
            {
                MessageBox.Show("Для каждой вариации должна быть ровно одна основная фотография");
                return false;
            }
        }

        return true;
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}