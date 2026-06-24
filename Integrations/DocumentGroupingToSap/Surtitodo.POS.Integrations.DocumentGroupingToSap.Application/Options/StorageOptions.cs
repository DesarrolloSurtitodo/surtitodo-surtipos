namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public string RequestsPath { get; init; } = string.Empty;

    public string ResponsesPath { get; init; } = string.Empty;
}
