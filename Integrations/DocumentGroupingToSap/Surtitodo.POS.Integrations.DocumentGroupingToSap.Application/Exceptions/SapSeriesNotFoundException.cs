namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;

public sealed class SapSeriesNotFoundException : Exception
{
    public SapSeriesNotFoundException(string warehouseCode, string numAtCard)
        : base($"Excepción para el documento {numAtCard}: No se encontró una serie de SAP configurada para la bodega '{warehouseCode}'.")
    {
    }
}
