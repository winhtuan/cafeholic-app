using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CAFEHOLIC.DAO;
using CAFEHOLIC.Model;
using CAFEHOLIC.Utils;

namespace CAFEHOLIC.Service;

public class PayOSPaymentResponse
{
    public string checkoutUrl { get; set; }
    public long orderCode { get; set; }
}

public class Payment
{
    private readonly string clientId;
    private readonly string apiKey;
    private readonly string checksumKey;
    private readonly string apiUrl = "https://api-merchant.payos.vn/v2/payment-requests";
    private readonly HttpClient httpClient;

    public Payment()
    {
        clientId = AppConfig.Get("PayOS:ClientId");
        apiKey = AppConfig.Get("PayOS:ApiKey");
        checksumKey = AppConfig.Get("PayOS:ChecksumKey");
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
        httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
    }

    public async Task<string?> CreatePaymentAsync(long orderCode, int amount, string description, string returnUrl, string cancelUrl)
    {
        try
        {
            var payload = new
            {
                orderCode = orderCode,
                amount = amount,
                description = description,
                returnUrl = returnUrl,
                cancelUrl = cancelUrl
            };

            var response = await httpClient.PostAsJsonAsync(apiUrl, payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PayOSPaymentResponse>();

            return result?.checkoutUrl;
        }
        catch (Exception ex)
        {
            Logger.Error(nameof(Payment), "Lỗi khi tạo thanh toán PayOS", ex);
            return null;
        }
    }

    public void OpenPaymentUrl(string? url)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
    }

}