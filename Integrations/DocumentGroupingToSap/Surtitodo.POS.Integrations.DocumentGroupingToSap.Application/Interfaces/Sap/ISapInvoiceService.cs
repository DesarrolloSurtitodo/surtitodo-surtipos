using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;

public interface ISapInvoiceService
{
    Task<SapInvoiceResult> CreateInvoiceAsync(SapInvoiceRequest request, CancellationToken cancellationToken);
}
