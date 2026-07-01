using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Navigation;

public partial class NavigationItem : UserControl
{
    public NavigationItem()
    {
        InitializeComponent();
        Loaded += (_, _) => Refresh();
    }

    #region DependencyProperties

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(NavigationItem),
            new PropertyMetadata(string.Empty));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(SyncModuleStatus), typeof(NavigationItem),
            new PropertyMetadata(SyncModuleStatus.Stopped, OnStatusChanged));
    public SyncModuleStatus Status
    {
        get => (SyncModuleStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(NavigationItem),
            new PropertyMetadata(false, OnSelectionChanged));
    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    #endregion

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((NavigationItem)d).Refresh();

    private static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((NavigationItem)d).Refresh();

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        Loaded += (_, _) => Refresh();
    }

    private void Refresh()
    {
        if (!IsLoaded) return;

        // Color del punto de estado
        (StatusEllipse.Fill, SubtitleText.Text) = Status switch
        {
            SyncModuleStatus.Running => ((Brush)FindResource("SuccessBrush"), "Ejecutándose"),
            SyncModuleStatus.Paused => ((Brush)FindResource("WarningBrush"), "Pausado"),
            SyncModuleStatus.Error => ((Brush)FindResource("ErrorBrush"), "Con errores"),
            _ => ((Brush)FindResource("StoppedBrush"), "Detenido"),
        };

        // Selección: fondo elevado + borde izquierdo rojo
        if (IsSelected)
        {
            Container.Background = (Brush)FindResource("CardBackgroundBrush");
            Container.BorderBrush = (Brush)FindResource("PrimaryBrush");
            Container.BorderThickness = new Thickness(2, 0, 0, 0);
            TitleText.FontWeight = FontWeights.Bold;
            TitleText.Foreground = (Brush)FindResource("TextBrush");
        }
        else
        {
            Container.Background = Brushes.Transparent;
            Container.BorderThickness = new Thickness(0);
            TitleText.FontWeight = FontWeights.Medium;
            TitleText.Foreground = (Brush)FindResource("TextSecondaryBrush");
        }
    }
}
