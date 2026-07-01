using System.Windows.Controls;
using System.Windows.Input;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Views.Controls.Layout;

public partial class Sidebar : UserControl
{
    public event Action<string>? ModuleSelected;

    public Sidebar()
    {
        InitializeComponent();

        ItemSap.MouseLeftButtonUp += (_, _) => Select("doc-grouping-to-sap");
        ItemEngine.MouseLeftButtonUp += (_, _) => Select("doc-grouping-engine");
    }

    private void Select(string moduleId)
    {
        ItemSap.IsSelected = moduleId == "doc-grouping-to-sap";
        ItemEngine.IsSelected = moduleId == "doc-grouping-engine";

        ModuleSelected?.Invoke(moduleId);
    }

    /// <summary>Permite que MainWindow actualice el estado mostrado en el sidebar.</summary>
    public void UpdateStatus(string moduleId, Models.SyncModuleStatus status)
    {
        if (moduleId == "doc-grouping-to-sap") ItemSap.Status = status;
        if (moduleId == "doc-grouping-engine") ItemEngine.Status = status;
    }
}