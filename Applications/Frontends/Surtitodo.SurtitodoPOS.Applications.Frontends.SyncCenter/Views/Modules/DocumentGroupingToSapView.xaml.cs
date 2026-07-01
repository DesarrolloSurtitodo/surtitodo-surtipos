using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Worker;
using Surtitodo.POS.Shared.SharedProcessMonitor;
using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;
using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Console;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Modules;

public partial class DocumentGroupingToSapView : UserControl
{
    // ── Estado ────────────────────────────────────────────────────────────────
    private SyncModuleStatus _status = SyncModuleStatus.Stopped;
    private readonly System.Timers.Timer _countdownTimer = new(1000);
    private int _secondsUntilNext;

    // ── Módulo real ───────────────────────────────────────────────────────────
    private readonly DocumentGroupingToSapModule _module;

    // ── Métricas ──────────────────────────────────────────────────────────────
    private int _procesados, _correctos, _errores;

    // ── Brushes ───────────────────────────────────────────────────────────────
    private Brush? _brushMuted;
    private Brush? _brushRed;

    // ── Evento hacia MainWindow ───────────────────────────────────────────────
    public event Action<SyncModuleStatus>? StatusChanged;

    public DocumentGroupingToSapView()
    {
        InitializeComponent();

        _module = new DocumentGroupingToSapModule(LoadConfiguration());

        // Una sola vez — el reader vive toda la vida de la vista
        _ = ConsumeEventsAsync(_module.EventChannel);

        _countdownTimer.Elapsed += (_, _) => OnCountdownTick();

        BtnEjecutar.Click += async (_, _) => await Ejecutar();
        BtnPausar.Click += async (_, _) => await PausarReanudar();
        BtnReiniciar.Click += async (_, _) => await Reiniciar();
        BtnLogs.Click += (_, _) => AbrirLogs();
        BtnLimpiar.Click += (_, _) => Console.Clear();

        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _brushMuted = (Brush)FindResource("TextMutedBrush");
        _brushRed = (Brush)FindResource("PrimaryBrush");
        RefreshButtons();
    }

    // ── Acciones ──────────────────────────────────────────────────────────────

    private async Task Ejecutar()
    {
        await _module.StartAsync();
        SetStatus(SyncModuleStatus.Running);
        StartCountdown();
    }

    private async Task PausarReanudar()
    {
        if (_status == SyncModuleStatus.Running)
        {
            await _module.PauseAsync();
            SetStatus(SyncModuleStatus.Paused);
            _countdownTimer.Stop();
        }
        else if (_status == SyncModuleStatus.Paused)
        {
            await _module.ResumeAsync();
            SetStatus(SyncModuleStatus.Running);
            _countdownTimer.Start();
        }
    }

    private async Task Reiniciar()
    {
        await _module.StopAsync();
        await _module.StartAsync();
        SetStatus(SyncModuleStatus.Running);
        StartCountdown();
    }

    private void AbrirLogs()
    {
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        Directory.CreateDirectory(path);
        Process.Start(new ProcessStartInfo("explorer.exe", path) { UseShellExecute = true });
    }

    // ── Consumo del canal de eventos ──────────────────────────────────────────

    private async Task ConsumeEventsAsync(WorkerEventChannel channel)
    {
        await foreach (var evt in channel.Reader.ReadAllAsync())
        {
            // Las métricas no van a consola — solo actualizan las cards
            if (evt.Type == WorkerEventType.Metrics)
            {
                Dispatcher.Invoke(() =>
                {
                    CardProcesados.Value = (evt.Procesados ?? 0).ToString();
                    CardCorrectos.Value = (evt.Correctos ?? 0).ToString();
                    CardErrores.Value = (evt.Errores ?? 0).ToString();
                    CardPendientes.Value = (evt.Pendientes ?? 0).ToString();
                });
                continue;
            }

            var type = evt.Type switch
            {
                WorkerEventType.Success => ConsoleEntryType.Success,
                WorkerEventType.Error => ConsoleEntryType.Error,
                WorkerEventType.Warning => ConsoleEntryType.Warning,
                WorkerEventType.Started => ConsoleEntryType.Started,
                WorkerEventType.Paused => ConsoleEntryType.Paused,
                WorkerEventType.Stopped => ConsoleEntryType.Stopped,
                _ => ConsoleEntryType.Info,
            };

            Console.AppendEntry(type, evt.Message);

            Dispatcher.Invoke(() =>
                TxtUltimaEjecucion.Text = evt.Timestamp.ToString("hh:mm:ss tt"));
        }
    }

    // ── Helpers UI ────────────────────────────────────────────────────────────

    private void SetStatus(SyncModuleStatus status)
    {
        _status = status;
        Dispatcher.Invoke(() =>
        {
            StatusBadgeControl.Status = status;
            StatusChanged?.Invoke(status);
            RefreshButtons();

            if (status == SyncModuleStatus.Stopped)
            {
                TxtProximaEjecucion.Foreground = _brushMuted ?? TxtProximaEjecucion.Foreground;
                TxtProximaEjecucion.Text = "--";
            }
        });
    }

    private void RefreshButtons()
    {
        BtnEjecutar.IsEnabled = _status == SyncModuleStatus.Stopped;
        BtnPausar.IsEnabled = _status is SyncModuleStatus.Running or SyncModuleStatus.Paused;
        BtnReiniciar.IsEnabled = _status != SyncModuleStatus.Stopped;

        BtnPausar.Content = BuildBtnContent(
            _status == SyncModuleStatus.Paused ? "▶" : "⏸",
            _status == SyncModuleStatus.Paused ? "Reanudar" : "Pausar");
    }

    private static StackPanel BuildBtnContent(string icon, string label)
    {
        var sp = new StackPanel { Orientation = Orientation.Horizontal };
        sp.Children.Add(new TextBlock
        {
            Text = icon,
            FontSize = 11,
            Margin = new Thickness(0, 0, 7, 0),
            VerticalAlignment = VerticalAlignment.Center
        });
        sp.Children.Add(new TextBlock
        {
            Text = label,
            VerticalAlignment = VerticalAlignment.Center
        });
        return sp;
    }

    private void RefreshMetrics()
    {
        Dispatcher.Invoke(() =>
        {
            CardProcesados.Value = _procesados.ToString();
            CardCorrectos.Value = _correctos.ToString();
            CardErrores.Value = _errores.ToString();
        });
    }

    private void StartCountdown()
    {
        _secondsUntilNext = _module.IntervalSeconds;
        _countdownTimer.Start();
    }

    private void OnCountdownTick()
    {
        if (_status == SyncModuleStatus.Running && _secondsUntilNext > 0)
            _secondsUntilNext--;

        Dispatcher.Invoke(() =>
        {
            if (_status == SyncModuleStatus.Stopped) return;

            TxtProximaEjecucion.Foreground = _secondsUntilNext > 0
                ? (_brushRed ?? TxtProximaEjecucion.Foreground)
                : (_brushMuted ?? TxtProximaEjecucion.Foreground);

            TxtProximaEjecucion.Text = _secondsUntilNext > 0
                ? $"{_secondsUntilNext}s"
                : "Ejecutando...";
        });
    }

    private void SetBatch(int current, int total)
    {
        Dispatcher.Invoke(() =>
        {
            TxtBatchInfo.Text = $"{current}/{total} documentos";
            if (BatchProgressBar.Parent is Border parent && parent.ActualWidth > 0)
            {
                double pct = total > 0 ? (double)current / total : 0;
                BatchProgressBar.Width = pct * (parent.ActualWidth - 28);
            }
        });
    }

    // ── Configuración ─────────────────────────────────────────────────────────

    private static IConfiguration LoadConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
}