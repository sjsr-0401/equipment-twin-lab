using System.Windows;
using EquipmentTwin.Hmi.Wpf.ViewModels;

namespace EquipmentTwin.Hmi.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new OperatorConsoleViewModel();
    }
}
