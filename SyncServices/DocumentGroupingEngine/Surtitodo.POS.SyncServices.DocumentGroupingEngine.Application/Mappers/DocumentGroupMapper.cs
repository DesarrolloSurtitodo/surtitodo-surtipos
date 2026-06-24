using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Grouping;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Source;
using Surtitodo.POS.SyncServices.DocumentGroupingEngine.Domain.Target;

namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Mappers;

public static class DocumentGroupMapper
{
    private static readonly Dictionary<string, string> DocTypeMap = new()
    {
        { "VTI", "VTI_AG" },
        { "VTD", "VTD_AG" }
    };

    public static DocumentAgroup ToTarget(
        DocumentGroup group,
        IEnumerable<DocumentsLines> lines,
        string numAtCard)
    {
        if (!DocTypeMap.TryGetValue(group.TIPDOC, out var agroupType))
            throw new InvalidOperationException($"Tipo de documento no mapeado: {group.TIPDOC}");

        var now = DateTime.Now;

        return new DocumentAgroup
        {
            AgroupDate = DateOnly.FromDateTime(now).ToDateTime(TimeOnly.MinValue),
            AgroupHour = now.TimeOfDay,
            AgroupDateTime = now,
            DocumentAgroupType = agroupType,
            WarehouseCode = group.BOCODI,
            CashBoxCode = group.CACODI,
            DocDate = group.Members.Min(d => d.TIDATA),
            CardCode = group.Members.First().CLCODI,
            NumAtCard = numAtCard,

            // Líneas — hijas de DocumentAgroup, EF resuelve la FK
            Lines = lines.Select(l => new DocumentAgroupLines
            {
                WarehouseCode = l.BOCODI,
                ItemCode = l.ARCODI,
                Quantity = l.TLQTT,
                Price = l.TLTOT,
                DiscountPercent = l.TLPDTE,
                TaxCode = l.CODIGO_IVA
            }).ToList(),

            // Trazas — hijas de DocumentAgroup, EF resuelve la FK
            // DocumentAgroupId no se asigna aquí — EF lo resuelve por navegación
            Traces = group.Members.Select(m => new DocumentAgroupTrace
            {
                BOCODI = m.BOCODI,
                CACODI = m.CACODI,
                TIPDOC = m.TIPDOC,
                TICODI = m.TICODI,
                TITOT = m.TITOT,
                TracedAt = now
            }).ToList()
        };
    }

    public static string BuildNumAtCard(string tipdoc, string bocodi, string cacodi)
    {
        if (!DocTypeMap.TryGetValue(tipdoc, out var agroupType))
            throw new InvalidOperationException($"Tipo de documento no mapeado: {tipdoc}");

        var now = DateTime.Now;
        var random = new Random();

        var uniquePart = $"{now:yyyyMMdd}-{now:HHmmss}-{now.Millisecond:D3}-{random.Next(1000, 9999)}";

        return $"{agroupType}-{bocodi}-{cacodi}-{uniquePart}";
    }
}