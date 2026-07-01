using System.Windows;
using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;
using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Modules;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views;

public partial class MainWindow : Window
{
    private DocumentGroupingToSapView? _sapView;
    private DocumentGroupingEngineView? _engineView;

    public MainWindow()
    {
        InitializeComponent();

        MainSidebar.ModuleSelected += OnModuleSelected;

        // Carga inicial: el primer módulo del sidebar
        ShowEngineModule();
    }

    private void OnModuleSelected(string moduleId)
    {
        switch (moduleId)
        {
            case "doc-grouping-to-sap":
                ShowSapModule();
                break;
            case "doc-grouping-engine":
                ShowEngineModule();
                break;
        }
    }

    private void ShowSapModule()
    {
        _sapView ??= CreateSapView();
        ModuleContent.Content = _sapView;

        MainHeader.Title = "Integración de documentos con SAP B1";
        MainHeader.Subtitle = "Integración de facturas y devoluciones agrupadas con SAP Business One";
        MainHeader.HeaderStatusBadge.Status = SyncModuleStatus.Stopped; // se actualizará con el evento
    }

    private void ShowEngineModule()
    {
        _engineView ??= CreateEngineView();
        ModuleContent.Content = _engineView;

        MainHeader.Title = "Agrupación de documentos";
        MainHeader.Subtitle = "Agrupa documentos pendientes desde el POS origen";
        MainHeader.HeaderStatusBadge.Status = SyncModuleStatus.Stopped;
    }

    private DocumentGroupingToSapView CreateSapView()
    {
        var view = new DocumentGroupingToSapView();
        view.StatusChanged += status =>
            Dispatcher.Invoke(() =>
            {
                MainHeader.HeaderStatusBadge.Status = status;
                MainSidebar.UpdateStatus("doc-grouping-to-sap", status);
            });
        return view;
    }

    private DocumentGroupingEngineView CreateEngineView()
    {
        var view = new DocumentGroupingEngineView();
        view.StatusChanged += status =>
            Dispatcher.Invoke(() =>
            {
                MainHeader.HeaderStatusBadge.Status = status;
                MainSidebar.UpdateStatus("doc-grouping-engine", status);
            });
        return view;
    }
}