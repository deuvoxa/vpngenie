using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public class AdminUsersMenu(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    CancellationToken cancellationToken)
{
    public async Task Handle()
    {
        var users = await userService.GetLastUsersAsync();
        
        var text = "*Меню пользователей*";

        var keyboard = new KeyboardBuilder()
            .WithButton("Получить полльзователя", "admin-get-user")
            .WithButton("Рассылка", "admin-newsletter")
            .WithBackAdmin().Build();
        
        await EditMessage(text, keyboard);
    }

    private async Task EditMessage(string text, InlineKeyboardMarkup keyboard)
    {
        await botClient.EditMessageTextAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}