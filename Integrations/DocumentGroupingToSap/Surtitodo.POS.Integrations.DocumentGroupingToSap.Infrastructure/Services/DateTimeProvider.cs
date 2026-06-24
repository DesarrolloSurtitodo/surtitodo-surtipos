using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Services;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
}
