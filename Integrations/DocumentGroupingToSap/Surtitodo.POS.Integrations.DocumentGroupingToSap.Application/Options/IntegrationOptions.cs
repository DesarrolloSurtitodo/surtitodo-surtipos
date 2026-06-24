namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;

public sealed class IntegrationOptions
{
    public const string SectionName = "Integration";

    public int BatchSize { get; init; }

    public int MaxParallelism { get; init; }

    public int ExecutionIntervalSeconds { get; init; }

    public int HttpTimeoutSeconds { get; init; }
}
