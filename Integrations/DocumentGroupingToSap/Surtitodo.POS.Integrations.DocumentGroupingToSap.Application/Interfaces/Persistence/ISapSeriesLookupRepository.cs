namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Persistence
{
    public interface ISapSeriesLookupRepository
    {
        Task<int?> GetSeriesNumberAsync(string warehouseCode, CancellationToken cancellationToken);
    }
}
