using DomDivan.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для SupplierViewWindow.xaml
/// </summary>
public partial class SupplierViewWindow : Window
{
    private readonly DomDivanContext _context;
    private ObservableCollection<Supplier> _suppliers = new ObservableCollection<Supplier>();

    public SupplierViewWindow()
    {
        InitializeComponent();
        _context = new DomDivanContext();
        
        LoadSuppliers();
    }

    private void LoadSuppliers()
    {
        try
        {
            SuppliersListBox.ItemsSource = null;
            _suppliers = new ObservableCollection<Supplier>(_context.Suppliers.ToList());
            SuppliersListBox.ItemsSource = _suppliers;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка загрузки поставщиков: {ex.Message}", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        var editWindow = new EditSupplierWindow
        {
            Title = "Добавление поставщика"
        };

        if (editWindow.ShowDialog() == true)
        {
            try
            {
                var newSupplier = new Supplier
                {
                    CompanyName = editWindow.CompanyNameTextBox.Text,
                    ContactPerson = editWindow.ContactPersonTextBox.Text,
                    PhoneNumber = editWindow.PhoneNumberTextBox.Text
                };

                _context.Suppliers.Add(newSupplier);
                _context.SaveChanges();

                LoadSuppliers();
                SuppliersListBox.SelectedItem = newSupplier;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения поставщика: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (SuppliersListBox.SelectedItem is Supplier selectedSupplier)
        {
            var editWindow = new EditSupplierWindow
            {
                Title = "Редактирование поставщика",
                CompanyNameTextBox = { Text = selectedSupplier.CompanyName },
                ContactPersonTextBox = { Text = selectedSupplier.ContactPerson },
                PhoneNumberTextBox = { Text = selectedSupplier.PhoneNumber }
            };

            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    selectedSupplier.CompanyName = editWindow.CompanyNameTextBox.Text;
                    selectedSupplier.ContactPerson = editWindow.ContactPersonTextBox.Text;
                    selectedSupplier.PhoneNumber = editWindow.PhoneNumberTextBox.Text;

                    _context.Suppliers.Update(selectedSupplier);
                    _context.SaveChanges();

                    LoadSuppliers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка обновления поставщика: {ex.Message}", "Ошибка",
                                  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (SuppliersListBox.SelectedItem is Supplier selectedSupplier)
        {
            if (MessageBox.Show($"Удалить поставщика {selectedSupplier.CompanyName}?",
                "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    _context.Suppliers.Remove(selectedSupplier);
                    _context.SaveChanges();

                    LoadSuppliers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления поставщика: {ex.Message}", "Ошибка",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    private void SuppliersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool hasSelection = SuppliersListBox.SelectedItem != null;
        EditButton.IsEnabled = hasSelection;
        DeleteButton.IsEnabled = hasSelection;
    }
}