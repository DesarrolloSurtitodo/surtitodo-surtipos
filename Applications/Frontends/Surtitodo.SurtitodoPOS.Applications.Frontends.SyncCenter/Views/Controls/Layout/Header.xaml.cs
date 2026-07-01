using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;
using System.Windows;
using System.Windows.Controls;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Layout;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    public SyncModuleStatus ModuleStatus
    {
        get => HeaderStatusBadge.Status;
        set => HeaderStatusBadge.Status = value;
    }

    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(Header),
            new PropertyMetadata(string.Empty));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty SubtitleProperty =
        DependencyProperty.Register(
            nameof(Subtitle),
            typeof(string),
            typeof(Header),
            new PropertyMetadata(string.Empty));

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public static readonly DependencyProperty LastUpdateProperty =
        DependencyProperty.Register(
            nameof(LastUpdate),
            typeof(string),
            typeof(Header),
            new PropertyMetadata("--"));

    public string LastUpdate
    {
        get => (string)GetValue(LastUpdateProperty);
        set => SetValue(LastUpdateProperty, value);
    }
}