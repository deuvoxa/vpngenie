using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using vpngenie.API.Contracts;
using vpngenie.API.TelegramBot.Handlers;
using vpngenie.Application.Services;

namespace vpngenie.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentController(
    ITelegramBotClient botClient,
    UserService userService) : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> PaymentWebhook([FromBody] dynamic requestBody)
    {
        string json = requestBody.ToString();
        var paymentCallback = JsonConvert.DeserializeObject<PaymentNotification>(json);
        switch (paymentCallback.Event)
        {
            case "payment.succeeded":
                await SuccessfulPaymentHandle.Execute(botClient, userService, paymentCallback.Object.Amount.Value,
                    paymentCallback.Object.Metadata.TelegramId, Guid.Parse(paymentCallback.Object.Id));
                break;
            case "payment.waiting_for_capture":
                break;
            case "payment.canceled":
                break;
        }

        return Ok();
    }
}