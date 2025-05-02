using DomDivan.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Button = System.Windows.Controls.Button;
using ColorVariant = DomDivan.Models.ColorVariant;

namespace DomDivan;

/// <summary>
/// Логика взаимодействия для ParametersWindow.xaml
/// </summary>
public partial class ParametersWindow : Window
{
    private readonly DomDivanContext _context;

    public ObservableCollection<Cloth> Cloths { get; set; }
    public ObservableCollection<ColorVariant> Colors { get; set; }
    public ObservableCollection<Filler> Fillers { get; set; }
    public ObservableCollection<FoldingMechanism> Mechanisms { get; set; }
    public ObservableCollection<SofaType> SofaTypes { get; set; }

    public ParametersWindow()
    {
        _context = new DomDivanContext();
        InitializeComponent();
        LoadData();
        DataContext = this;
    }

    private void LoadData()
    {
        Cloths = new ObservableCollection<Cloth>(_context.Cloths.ToList());
        Colors = new ObservableCollection<ColorVariant>(_context.Colors.ToList());
        Fillers = new ObservableCollection<Filler>(_context.Fillers.ToList());
        Mechanisms = new ObservableCollection<FoldingMechanism>(_context.FoldingMechanisms.ToList());
        SofaTypes = new ObservableCollection<SofaType>(_context.SofaTypes.ToList());

        ClothsDataGrid.ItemsSource = Cloths;
        ColorsDataGrid.ItemsSource = Colors;
        FillersDataGrid.ItemsSource = Fillers;
        MechanismsDataGrid.ItemsSource = Mechanisms;
        SofaTypesDataGrid.ItemsSource = SofaTypes;
    }

    #region Общие методы
    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        new AdminProductsWindow().Show();
        this.Close();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        _context.Dispose();
    }
    #endregion

    #region Обработчики для тканей
    private void AddCloth_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewClothTextBox.Text))
        {
            var newCloth = new Cloth { Name = NewClothTextBox.Text };
            _context.Cloths.Add(newCloth);
            _context.SaveChanges();

            Cloths.Add(newCloth);
            NewClothTextBox.Clear();
        }
    }

    private void EditCloth_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is Cloth selectedCloth)
        {
            var editWindow = new EditParameterWindow("Редактирование ткани", selectedCloth.Name);
            if (editWindow.ShowDialog() == true)
            {
                var cloth = _context.Cloths.Find(selectedCloth.Id);
                if (cloth != null)
                {
                    cloth.Name = editWindow.ParameterValue;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }
    }

    private void DeleteCloth_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is Cloth cloth)
        {
            _context.Cloths.Remove(cloth);
            _context.SaveChanges();
            Cloths.Remove(cloth);
        }
    }

    private void ClothsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ClothsDataGrid.SelectedItem is Cloth selectedCloth)
        {
            EditCloth_Click(new Button { Tag = selectedCloth }, null);
        }
    }
    #endregion

    #region Обработчики для цветов
    private void AddColor_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewColorNameTextBox.Text) &&
            !string.IsNullOrWhiteSpace(NewColorHexTextBox.Text))
        {
            var newColor = new ColorVariant
            {
                Name = NewColorNameTextBox.Text,
                HexID = NewColorHexTextBox.Text
            };
            _context.Colors.Add(newColor);
            _context.SaveChanges();

            Colors.Add(newColor);
            NewColorNameTextBox.Clear();
            NewColorHexTextBox.Clear();
        }
    }

    private void EditColor_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is ColorVariant selectedColor)
        {
            var editWindow = new EditColorWindow(selectedColor);
            if (editWindow.ShowDialog() == true)
            {
                var color = _context.Colors.Find(selectedColor.Id);
                if (color != null)
                {
                    color.Name = editWindow.ColorName;
                    color.HexID = editWindow.ColorHex;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }
    }

    private void DeleteColor_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is ColorVariant color)
        {
            _context.Colors.Remove(color);
            _context.SaveChanges();
            Colors.Remove(color);
        }
    }

    private void ColorsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (ColorsDataGrid.SelectedItem is ColorVariant selectedColor)
        {
            EditColor_Click(new Button { Tag = selectedColor }, null);
        }
    }
    #endregion

    #region Обработчики для наполнителей
    private void AddFiller_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewFillerTextBox.Text))
        {
            var newFiller = new Filler { Name = NewFillerTextBox.Text };
            _context.Fillers.Add(newFiller);
            _context.SaveChanges();

            Fillers.Add(newFiller);
            NewFillerTextBox.Clear();
        }
    }

    private void EditFiller_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is Filler selectedFiller)
        {
            var editWindow = new EditParameterWindow("Редактирование наполнителя", selectedFiller.Name);
            if (editWindow.ShowDialog() == true)
            {
                var filler = _context.Fillers.Find(selectedFiller.Id);
                if (filler != null)
                {
                    filler.Name = editWindow.ParameterValue;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }
    }

    private void DeleteFiller_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is Filler filler)
        {
            _context.Fillers.Remove(filler);
            _context.SaveChanges();
            Fillers.Remove(filler);
        }
    }

    private void FillersDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (FillersDataGrid.SelectedItem is Filler selectedFiller)
        {
            EditFiller_Click(new Button { Tag = selectedFiller }, null);
        }
    }
    #endregion

    #region Обработчики для механизмов
    private void AddMechanism_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewMechanismTextBox.Text))
        {
            var newMechanism = new FoldingMechanism { Name = NewMechanismTextBox.Text };
            _context.FoldingMechanisms.Add(newMechanism);
            _context.SaveChanges();

            Mechanisms.Add(newMechanism);
            NewMechanismTextBox.Clear();
        }
    }

    private void EditMechanism_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is FoldingMechanism selectedMechanism)
        {
            var editWindow = new EditParameterWindow("Редактирование механизма", selectedMechanism.Name);
            if (editWindow.ShowDialog() == true)
            {
                var mechanism = _context.FoldingMechanisms.Find(selectedMechanism.Id);
                if (mechanism != null)
                {
                    mechanism.Name = editWindow.ParameterValue;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }
    }

    private void DeleteMechanism_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is FoldingMechanism mechanism)
        {
            _context.FoldingMechanisms.Remove(mechanism);
            _context.SaveChanges();
            Mechanisms.Remove(mechanism);
        }
    }

    private void MechanismsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (MechanismsDataGrid.SelectedItem is FoldingMechanism selectedMechanism)
        {
            EditMechanism_Click(new Button { Tag = selectedMechanism }, null);
        }
    }
    #endregion

    #region Обработчики для типов диванов
    private void AddSofaType_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(NewSofaTypeTextBox.Text))
        {
            var newSofaType = new SofaType { Name = NewSofaTypeTextBox.Text };
            _context.SofaTypes.Add(newSofaType);
            _context.SaveChanges();

            SofaTypes.Add(newSofaType);
            NewSofaTypeTextBox.Clear();
        }
    }

    private void EditSofaType_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is SofaType selectedSofaType)
        {
            var editWindow = new EditParameterWindow("Редактирование типа дивана", selectedSofaType.Name);
            if (editWindow.ShowDialog() == true)
            {
                var sofaType = _context.SofaTypes.Find(selectedSofaType.Id);
                if (sofaType != null)
                {
                    sofaType.Name = editWindow.ParameterValue;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }
    }

    private void DeleteSofaType_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is SofaType sofaType)
        {
            _context.SofaTypes.Remove(sofaType);
            _context.SaveChanges();
            SofaTypes.Remove(sofaType);
        }
    }

    private void SofaTypesDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (SofaTypesDataGrid.SelectedItem is SofaType selectedSofaType)
        {
            EditSofaType_Click(new Button { Tag = selectedSofaType }, null);
        }
    }
    #endregion
}