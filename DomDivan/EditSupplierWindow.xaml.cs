using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для EditSupplierWindow.xaml
/// </summary>
public partial class EditSupplierWindow : Window
{
    public EditSupplierWindow()
    {
        InitializeComponent();
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text) ||
            string.IsNullOrWhiteSpace(ContactPersonTextBox.Text) ||
            string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
        {
            MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}