using System.Windows;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для EditParameterWindow.xaml
/// </summary>
public partial class EditParameterWindow : Window
{
    public string WindowTitle { get; }
    public string ParameterName { get; }
    public string ParameterValue { get; set; }

    public EditParameterWindow(string windowTitle, string currentValue)
    {
        WindowTitle = windowTitle;
        ParameterName = windowTitle.Replace("Редактирование ", "") + ':';
        ParameterValue = currentValue;

        InitializeComponent();
        DataContext = this;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}