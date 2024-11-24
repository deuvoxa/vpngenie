using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using vpngenie.Application.Services;

namespace vpngenie.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class UserController(
    IConfiguration configuration,
    ITelegramBotClient botClient,
    UserService userService,
    WireGuardService wireGuardService) : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> Profile()
    {
        var telegramId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (telegramId == null)
        {
            return Unauthorized("Telegram ID not found in token.");
        }

        var user = await userService.GetUserByTelegramIdAsync(long.Parse(telegramId));
        
        return Ok(user);
    }

    [HttpPost("renew-subscription")]
    public async Task<IActionResult> RenewSubscription([FromBody] EmailRequest email)
        {
        var telegramId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var yookassa = new YookassaClient(configuration);
        var paymentUrl = await yookassa.CreatePaymentAsync(129.0m, "Подписка на VPN Genie (31 день)",
            "http:/localhost:3000", long.Parse(telegramId), Service.ValidateEmail(email.Email));
        return Ok(paymentUrl);
    }
    
    // [HttpGet("get-config")]
    // public async Task<IActionResult> GetConfig()
    // {
    //     await wireGuardService.CreateWireGuardConfig()
    //     return Ok();
    // }
}

public record EmailRequest(string Email);