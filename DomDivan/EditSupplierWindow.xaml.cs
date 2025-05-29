using DomDivan.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для EditSupplierWindow.xaml
/// </summary>
public partial class EditSupplierWindow : Window, INotifyPropertyChanged
{
    private readonly Supplier _supplierToEdit;
    public bool _isEditMode;

    public Supplier Supplier { get; set; }
    public string WindowTitle => _isEditMode ? "Редактирование поставщика" : "Добавление нового поставщика";

    public EditSupplierWindow(Supplier supplier = null)
    {
        InitializeComponent();

        if (supplier != null)
        {
            // Режим редактирования
            _supplierToEdit = supplier;
            _isEditMode = true;

            Supplier = new Supplier() 
            {
                CompanyName = supplier.CompanyName,
                ContactPerson = supplier.ContactPerson,
                PhoneNumber = supplier.PhoneNumber
            };     
        }
        else
        {
            // Режим создания нового
            _isEditMode = false;
        }

        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(ContactPersonTextBox.Text) ||
            string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
        {
            MessageBox.Show("Все поля должны быть заполнены!", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using (var context = new DomDivanContext())
            {
                if (_isEditMode)
                {
                    // Обновляем существующего поставщика
                    var supplierInDb = context.Suppliers.Find(_supplierToEdit.Id);
                    if (supplierInDb != null)
                    {
                        supplierInDb.CompanyName = CompanyNameTextBox.Text;
                        supplierInDb.ContactPerson = ContactPersonTextBox.Text;
                        supplierInDb.PhoneNumber = PhoneNumberTextBox.Text;

                        context.Suppliers.Update(supplierInDb);
                    }
                }
                else
                {
                    // Добавляем нового поставщика
                    context.Suppliers.Add(new Supplier
                    {
                        CompanyName = CompanyNameTextBox.Text,
                        ContactPerson = ContactPersonTextBox.Text,
                        PhoneNumber = PhoneNumberTextBox.Text
                    });
                }

                context.SaveChanges();
            }

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                          MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}