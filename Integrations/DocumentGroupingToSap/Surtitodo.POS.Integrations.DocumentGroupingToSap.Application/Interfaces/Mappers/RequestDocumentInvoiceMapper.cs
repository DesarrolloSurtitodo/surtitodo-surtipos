using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Entities;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Mappers
{
    public static class RequestDocumentInvoiceMapper
    {
        public static SapInvoiceRequest ToRequest(DocumentAgroup document, int series)
        {
            return new SapInvoiceRequest
            {
                CardCode = document.CardCode,
                DocDate = document.DocDate,
                DocDueDate = document.DocDate,
                NumAtCard = document.NumAtCard,
                Comments = "Integración hecha desde el área de tecnología",
                Series = series,
                DocumentLines = document.Lines.Select(l => new SapInvoiceLineRequest
                {
                    WarehouseCode = "301", //l.WarehouseCode,
                    DiscountPercent = l.DiscountPercent,
                    ItemCode = l.ItemCode,
                    Price = l.Price,
                    Quantity = l.Quantity,
                    TaxCode = l.TaxCode
                }).ToList(),
            };
        }
    }
}
