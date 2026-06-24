using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Interfaces.Sap;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Requests;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Responses;
using Surtitodo.POS.Integrations.DocumentGroupingToSap.Application.Models.Results;
using System.Net.Http.Json;
using System.Text.Json;

namespace Surtitodo.POS.Integrations.DocumentGroupingToSap.Infrastructure.Sap;

public sealed class SapInvoiceService(IHttpClientFactory httpClientFactory, ISapSessionManager sessionManager) : ISapInvoiceService
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ISapSessionManager _sessionManager = sessionManager;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<SapInvoiceResult> CreateInvoiceAsync(SapInvoiceRequest request, CancellationToken cancellationToken)
    {
        var requestJson = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);

        var sessionId = await _sessionManager.GetSessionAsync(cancellationToken);

        var client = _httpClientFactory.CreateClient("SapServiceLayer");
        client.DefaultRequestHeaders.Remove("Cookie");
        client.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={sessionId}");

        var response = await client.PostAsJsonAsync("/b1s/v1/Invoices", request, cancellationToken);

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            var success = JsonSerializer.Deserialize<SapSuccessResponse>(responseJson, JsonOptions);

            return new SapInvoiceResult
            {
                Success = true,
                SapDocEntry = success?.DocEntry,
                SapDocNum = success?.DocNum,
                ErrorCode = null,
                ErrorMessage = "Integración exitosa",
                HttpCode = (int)response.StatusCode,
                HttpMessage = response.ReasonPhrase,
                RequestJson = requestJson,
                ResponseJson = responseJson
            };
        }
        else
        {
            var error = JsonSerializer.Deserialize<SapErrorResponse>(responseJson, JsonOptions);

            return new SapInvoiceResult
            {
                Success = false,
                SapDocEntry = null,
                SapDocNum = null,
                ErrorCode = error?.Error?.Code,
                ErrorMessage = error?.Error?.Message?.Value,
                HttpCode = (int)response.StatusCode,
                HttpMessage = response.ReasonPhrase,
                RequestJson = requestJson,
                ResponseJson = responseJson
            };
        }
    }
}
