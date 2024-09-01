using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace vpngenie.API;

public class YookassaClient
{
    private readonly HttpClient _httpClient;

    public YookassaClient(IConfiguration configuration)
    {
        var shopId = configuration["Yookassa:ShopId"] ?? throw new InvalidOperationException();
        var secretKey = configuration["Yookassa:SecretKey"] ?? throw new InvalidOperationException();
        _httpClient = new HttpClient();

        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{shopId}:{secretKey}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
    }

    public async Task<string> CreatePaymentAsync(decimal amount, string description, string returnUrl, long telegramId,
        string email)
    {
        const string requestUrl = "https://api.yookassa.ru/v3/payments";
        var requestData = new
        {
            amount = new
            {
                value = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                currency = "RUB"
            },
            confirmation = new
            {
                type = "redirect",
                return_url = returnUrl
            },
            capture = true,
            description,
            metadata = new { telegram_id = telegramId },
            receipt = new
            {
                items = new[]
                {
                    new
                    {
                        description = "Подписка (1 мес.)",
                        quantity = "1.00",
                        amount = new
                        {
                            value = "100.00",
                            currency = "RUB"
                        },
                        vat_code = 1
                    }
                },
                tax_system_code = 1,
                email
            }
        };

        var requestJson = JsonConvert.SerializeObject(requestData);
        var requestContent = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var idempotenceKey = Guid.NewGuid().ToString();

        _httpClient.DefaultRequestHeaders.Add("Idempotence-Key", idempotenceKey);

        var response = await _httpClient.PostAsync(requestUrl, requestContent);

        var responseContent = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        var responseData = JsonConvert.DeserializeObject<dynamic>(responseContent);

        return responseData?.confirmation.confirmation_url ?? throw new InvalidOperationException();
    }
}