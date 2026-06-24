namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

/// <summary>
/// Configuración de conexión hacia SAP HANA mediante ODBC.
/// </summary>
public sealed class HanaOptions
{
    public const string SectionName = "Hana";

    public string ConnectionString { get; init; } = string.Empty;
}
