using CommunityToolkit.Mvvm.ComponentModel;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Presentation.ViewModels;

partial class ModuleViewModel : ViewModelBase
{
    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private bool isSelected;

    [ObservableProperty]
    private bool isRunning;
}
