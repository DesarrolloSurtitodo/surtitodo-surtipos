namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Responses;

public sealed class SapErrorDetail
{
    public int Code { get; init; }
    public SapErrorMessage? Message { get; init; }
}
