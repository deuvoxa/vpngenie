using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using vpngenie.Application.Services;

namespace vpngenie.API.Controllers;

[Authorize(Policy = "Admin")]
[Route("api/[controller]")]
public class AdminController(
    ITelegramBotClient botClient,
    UserService userService) : ControllerBase
{
    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("get-activate-users")]
    public async Task<IActionResult> GetActivateUsers()
    {
        var users = await userService.GetAllUsersAsync();
        var activateUsers = users.Where(u => u.SubscriptionIsActive);
        return Ok(activateUsers);
    }

    [HttpGet("get-user-by-id")]
    public async Task<IActionResult> GetUserById(string request)
    {
        if (long.TryParse(request, out var telegramId))
        {
            var user = await userService.GetUserByTelegramIdAsync(telegramId);
            return Ok(user);
        }
        else
        {
            var users = await userService.GetAllUsersAsync();
            var user = users.FirstOrDefault(u => u.Username == request);
            if (user is null) return BadRequest(request);

            return Ok(user);
        }
    }
}