using System.Text.Json;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Sap
{
    public static class SapJsonOptions
    {
        public static readonly JsonSerializerOptions Default = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null
        };
    }
}
