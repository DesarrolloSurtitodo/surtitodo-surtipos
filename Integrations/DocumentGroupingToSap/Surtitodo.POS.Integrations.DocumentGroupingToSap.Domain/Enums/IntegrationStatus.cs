namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Domain.Enums;

/// <summary>
/// Estado de integración del documento hacia SAP.
/// </summary>
public enum IntegrationStatus
{
    Pending,
    Processing,
    Transferred,
    Error,
    Cancelled
}