namespace Surtitodo.POS.SyncServices.DocumentGroupingEngine.Application.Interfaces.Services
{
    public interface IErrorLogService
    {
        /// <summary>
        /// Crea el archivo de log y retorna su nombre para persistirlo en Source.
        /// </summary>
        Task<string> LogErrorAsync(
            string numAtCard,
            Exception exception,
            CancellationToken ct = default);
    }
}
