using System.Collections.ObjectModel;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Presentation.ViewModels;

partial class MainViewModel : ViewModelBase
{
    public ObservableCollection<ModuleViewModel> Modules { get; } = [];

    public MainViewModel()
    {
        Modules.Add(new ModuleViewModel
        {
            Name = "Document Grouping To SAP",
            Description = "Integra documentos agrupados con SAP Business One."
        });
    }
}
