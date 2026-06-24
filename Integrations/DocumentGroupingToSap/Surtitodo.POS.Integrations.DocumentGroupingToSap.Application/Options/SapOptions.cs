namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

public sealed class SapOptions
{
    public const string SectionName = "Sap";

    public string BaseUrl { get; init; } = string.Empty;

    public string CompanyDb { get; init; } = string.Empty;

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public int SessionTimeoutMinutes { get; init; }
    public bool IgnoreSslErrors { get; init; } = false;
}
