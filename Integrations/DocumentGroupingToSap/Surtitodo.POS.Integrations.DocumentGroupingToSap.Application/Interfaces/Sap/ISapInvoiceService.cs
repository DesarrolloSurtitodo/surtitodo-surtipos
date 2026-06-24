using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;

public interface ISapInvoiceService
{
    Task<CreateInvoiceResult> CreateInvoiceAsync(DocumentAgroup document, CancellationToken cancellationToken);
}
