using System.Windows;
using System.Windows.Controls;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Layout;

public partial class Footer : UserControl
{
    public Footer()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CopyrightProperty =
        DependencyProperty.Register(
            nameof(Copyright),
            typeof(string),
            typeof(Footer),
            new PropertyMetadata("© Compañía Comercial Universal S.A.S."));

    public string Copyright
    {
        get => (string)GetValue(CopyrightProperty);
        set => SetValue(CopyrightProperty, value);
    }

    public static readonly DependencyProperty VersionProperty =
        DependencyProperty.Register(
            nameof(Version),
            typeof(string),
            typeof(Footer),
            new PropertyMetadata("v1.0.0"));

    public string Version
    {
        get => (string)GetValue(VersionProperty);
        set => SetValue(VersionProperty, value);
    }
}