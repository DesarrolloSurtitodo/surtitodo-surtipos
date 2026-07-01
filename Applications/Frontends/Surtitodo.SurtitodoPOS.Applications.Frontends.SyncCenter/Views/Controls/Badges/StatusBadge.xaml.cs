using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Badges;

public partial class StatusBadge : UserControl
{
    public StatusBadge()
    {
        InitializeComponent();

        Loaded += (_, _) => Refresh();
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(
            nameof(Status),
            typeof(SyncModuleStatus),
            typeof(StatusBadge),
            new PropertyMetadata(
                SyncModuleStatus.Stopped,
                OnStatusChanged));

    public SyncModuleStatus Status
    {
        get => (SyncModuleStatus)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    private static void OnStatusChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        ((StatusBadge)d).Refresh();
    }

    private void Refresh()
    {
        if (!IsLoaded)
            return;

        switch (Status)
        {
            case SyncModuleStatus.Running:

                StatusEllipse.Fill = (Brush)FindResource("SuccessBrush");
                StatusText.Text = "Ejecutándose";

                break;

            case SyncModuleStatus.Paused:

                StatusEllipse.Fill = (Brush)FindResource("WarningBrush");
                StatusText.Text = "Pausado";

                break;

            case SyncModuleStatus.Error:

                StatusEllipse.Fill = (Brush)FindResource("ErrorBrush");
                StatusText.Text = "Con errores";

                break;

            default:

                StatusEllipse.Fill = (Brush)FindResource("StoppedBrush");
                StatusText.Text = "Detenido";

                break;
        }
    }
}