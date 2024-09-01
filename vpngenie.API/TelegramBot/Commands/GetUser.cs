using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Commands;

public class GetUser
{
    public static async Task ExecuteAsync(
        ITelegramBotClient botClient, Message message, string[] parametrs,
        UserService userService, long ownerId, CancellationToken cancellationToken)
    {
        if (message.From!.Id != ownerId) return;
        var users = await userService.GetAllUsersAsync();
        var user = users.FirstOrDefault(u => u.Username == parametrs[0]);
        await botClient.SendTextMessageAsync(
            message.Chat.Id, 
            $"[{user.Username}](tg://user?id={user.TelegramId})",
            parseMode: ParseMode.MarkdownV2,
            cancellationToken: cancellationToken);
    }
}