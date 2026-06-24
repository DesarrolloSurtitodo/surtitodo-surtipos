using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Responses;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;

public interface ISapInvoiceService
{
    Task<SapInvoiceResponse> CreateInvoiceAsync(SapInvoiceRequest request, CancellationToken cancellationToken);
}
