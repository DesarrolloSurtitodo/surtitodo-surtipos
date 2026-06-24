using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence;

public interface ISapInvoiceLookupRepository
{
    /// <summary>
    /// Busca una factura SAP utilizando NumAtCard.
    /// </summary>
    Task<SapInvoiceLookupResult?> FindByNumAtCardAsync(string numAtCard, CancellationToken cancellationToken);
}

