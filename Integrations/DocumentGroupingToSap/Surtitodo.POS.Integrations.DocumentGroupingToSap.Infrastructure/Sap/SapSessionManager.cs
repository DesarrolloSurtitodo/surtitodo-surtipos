using Microsoft.Extensions.Options;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Exceptions;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Responses;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Sap;

public class SapSessionManager(IHttpClientFactory httpClientFactory, IOptions<SapOptions> options) : ISapSessionManager
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly SapOptions _options = options.Value;

    private string? _sessionId;
    private DateTime _expiresAt;

    public async Task<string> GetSessionAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_sessionId) && DateTime.UtcNow < _expiresAt) return _sessionId;        

        await LoginAsync(cancellationToken);

        return _sessionId!;
    }

    private async Task LoginAsync(CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("SapServiceLayer");

        var request = new
        {
            CompanyDB = _options.CompanyDb,
            UserName = _options.UserName,
            Password = _options.Password
        };

        var response = await client.PostAsJsonAsync(
            "/b1s/v1/Login", request,
            SapJsonOptions.Default,
            cancellationToken);

        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var error = JsonSerializer.Deserialize<SapErrorResponse>(responseBody, SapJsonOptions.Default);

            throw new SapLoginException(
                httpStatusCode: (int)response.StatusCode,
                sapErrorCode: error?.Error?.Code ?? 0,
                sapErrorMessage: error?.Error?.Message?.Value ?? "Error desconocido");
        }

        var loginResponse = JsonSerializer.Deserialize<SapLoginResponse>(responseBody, SapJsonOptions.Default);

        if (loginResponse is null)
            throw new InvalidOperationException("SAP Service Layer no retornó información de sesión.");

        _sessionId = loginResponse.SessionId;
        _expiresAt = DateTime.UtcNow.AddMinutes(loginResponse.SessionTimeout - 5);
    }
}
