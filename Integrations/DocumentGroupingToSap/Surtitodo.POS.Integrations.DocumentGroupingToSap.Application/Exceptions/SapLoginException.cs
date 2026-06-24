namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;

public sealed class SapLoginException : Exception
{
    public int SapErrorCode { get; }
    public string SapErrorMessage { get; }
    public int HttpStatusCode { get; }

    public SapLoginException(int httpStatusCode, int sapErrorCode, string sapErrorMessage)
        : base($"SAP Login fallido [{httpStatusCode}] - Código SAP: {sapErrorCode} - {sapErrorMessage}")
    {
        HttpStatusCode = httpStatusCode;
        SapErrorCode = sapErrorCode;
        SapErrorMessage = sapErrorMessage;
    }
}
