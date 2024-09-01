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
        
        var text = users.Aggregate("*Админ меню (Пользователи)*\n\n",
            (current, user) => current +
                               $"_Пользователь:_ [{user.Username}](tg://user?id={user.TelegramId})\n" +
                               $"_Заданных вопросов:_ `{user.Tickets.Count}`\n" +
                               $"_Подключён к серверу:_\n`{user.Server?.IpAddress ?? "Не подключён"}`\n" +
                               $"_Подписка:_ {user.SubscriptionEndDate}\n\n");

        var keyboard = new KeyboardBuilder()
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