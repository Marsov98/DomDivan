using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // Логика обработки нажатия кнопки "Войти"
        string username = UsernameTextBox.Text;
        string password = PasswordBox.Password.ToString();

        new MainMenuWindow().Show();
        this.Close();
    }

    private void GuestButton_Click(object sender, RoutedEventArgs e)
    {
        new CatalogWindow().Show();
        this.Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
