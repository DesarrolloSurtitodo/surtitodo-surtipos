using Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Models;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Surtitodo.SurtitodoPOS.Applications.Frontends.SyncCenter.Converters;

class SyncModuleStatusToBrushConverter : IValueConverter
{
    public object Convert(object value,
                      Type targetType,
                      object parameter,
                      CultureInfo culture)
    {
        return (SyncModuleStatus)value switch
        {
            SyncModuleStatus.Running => new SolidColorBrush(Color.FromRgb(46, 204, 113)),
            SyncModuleStatus.Paused => new SolidColorBrush(Color.FromRgb(241, 196, 15)),
            SyncModuleStatus.Error => new SolidColorBrush(Color.FromRgb(231, 76, 60)),
            _ => new SolidColorBrush(Color.FromRgb(149, 165, 166))
        };
    }

    public object ConvertBack(object value,
                              Type targetType,
                              object parameter,
                              CultureInfo culture)
        => throw new NotImplementedException();
}
