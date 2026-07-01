using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;
using System.Globalization;
using System.Windows.Data;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Converters;

class SyncModuleStatusToTextConverter : IValueConverter
{
    public object Convert(object value,
                      Type targetType,
                      object parameter,
                      CultureInfo culture)
    {
        return (SyncModuleStatus)value switch
        {
            SyncModuleStatus.Running => "Ejecutando",
            SyncModuleStatus.Paused => "Pausado",
            SyncModuleStatus.Error => "Error",
            _ => "Detenido"
        };
    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        => throw new NotImplementedException();
}
