using System.Windows;
using System.Windows.Media;
using ColorVariant = DomDivan.Models.ColorVariant;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для EditColorWindow.xaml
/// </summary>
public partial class EditColorWindow : Window
{
    public string ColorName { get; private set; }
    public string ColorHex { get; private set; }

    public EditColorWindow(ColorVariant color)
    {
        InitializeComponent();
        NameTextBox.Text = color.Name;
        HexTextBox.Text = color.HexID;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        ColorName = NameTextBox.Text;
        ColorHex = HexTextBox.Text;
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
