using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Console;

// ── Modelo de una línea de consola ────────────────────────────────────────────
public class ConsoleLogEntry
{
    public string TimeDisplay { get; set; } = string.Empty;
    public string TypeLabel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public Brush BadgeBackground { get; set; } = Brushes.Transparent;
    public Brush BadgeForeground { get; set; } = Brushes.White;
    public Brush MessageForeground { get; set; } = Brushes.White;
}

public enum ConsoleEntryType { Info, Success, Error, Warning, Started, Paused, Stopped }

// ── Control ───────────────────────────────────────────────────────────────────
public partial class ConsolePanel : UserControl
{
    public ConsolePanel()
    {
        InitializeComponent();
        LogItems.ItemsSource = _entries;
    }

    // ── Entries (API pública) ────────────────────────────────────────────────
    private readonly ObservableCollection<ConsoleLogEntry> _entries = [];

    public void AppendEntry(ConsoleEntryType type, string message)
    {
        var (badgeBg, badgeFg, msgFg, label) = type switch
        {
            ConsoleEntryType.Success => (Color("#163A24"), Color("#22C55E"), Color("#D1FAE5"), "OK"),
            ConsoleEntryType.Error => (Color("#3A1212"), Color("#EF4444"), Color("#FEE2E2"), "ERR"),
            ConsoleEntryType.Warning => (Color("#3A2A08"), Color("#F59E0B"), Color("#FEF3C7"), "WRN"),
            ConsoleEntryType.Started => (Color("#0F2A3A"), Color("#60A5FA"), Color("#DBEAFE"), "RUN"),
            ConsoleEntryType.Paused => (Color("#2A2208"), Color("#F59E0B"), Color("#FEF3C7"), "PSE"),
            ConsoleEntryType.Stopped => (Color("#1E1E22"), Color("#71717A"), Color("#A1A1AA"), "STP"),
            _ => (Color("#1A1A1E"), Color("#60A5FA"), Color("#E2E8F0"), "INF"),
        };

        Dispatcher.Invoke(() =>
        {
            _entries.Add(new ConsoleLogEntry
            {
                TimeDisplay = DateTime.Now.ToString("hh:mm:ss tt"),
                TypeLabel = label,
                Message = message,
                BadgeBackground = new SolidColorBrush(badgeBg),
                BadgeForeground = new SolidColorBrush(badgeFg),
                MessageForeground = new SolidColorBrush(msgFg),
            });

            // Cap 500 líneas
            while (_entries.Count > 500)
                _entries.RemoveAt(0);

            // Auto-scroll
            Scroller.ScrollToEnd();
        });
    }

    public void Clear() => Dispatcher.Invoke(_entries.Clear);

    private static System.Windows.Media.Color Color(string hex)
        => (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(hex);

    // ── DependencyProperty Title ─────────────────────────────────────────────
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ConsolePanel),
            new PropertyMetadata("CONSOLA DE ACTIVIDAD"));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // ── DependencyProperty Text (compatibilidad) ─────────────────────────────
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(ConsolePanel),
            new PropertyMetadata(string.Empty));

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
