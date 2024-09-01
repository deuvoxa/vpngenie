using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public class AdminUsersNewsletter(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        // var users = await userService.GetAllUsersAsync();
        const string text = "";
    }
}