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
        string password = PasswordBox.Text;


        using (var context = new DomDivanContext())
        {
            new MainMenuWindow().Show();
            this.Close();

            /*Product product = context.Products
                .Include(p => p.Category)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Color)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Cloth)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.SofaType)
                .Include(p => p.Variants)
                    .ThenInclude(v => v.Photos)
                .FirstOrDefault();

            var editor = new ProductEditorWindow(product);
            if (editor.ShowDialog() == true)
            {
                MessageBox.Show("Товар успешно сохранен");
            }*/
            //new ProductEditorWindow().Show();
            //this.Close();
        }

        /*else
        {
            MessageBox.Show("Неверный логин или пароль.");
        }*/
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
