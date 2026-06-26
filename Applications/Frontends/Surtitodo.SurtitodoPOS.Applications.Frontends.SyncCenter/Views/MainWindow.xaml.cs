using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Presentation.ViewModels;
using System.Windows;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainViewModel();
    }
}