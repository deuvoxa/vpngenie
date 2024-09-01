using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public static class AdminSubscriptionMenu
{
    public static async Task Main(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserService userService,
        CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var users = await userService.GetAllUsersAsync();
        var usersWithSubscription = users.Where(u => u.SubscriptionIsActive);
        await botClient.EditMessageTextAsync(
            chatId: message.Chat.Id,
            messageId: message.MessageId,
            text: $"""
                  *Админ меню (Подписки)*

                  _Всего пользователей:_ `{users.Count}`
                  _Пользователи с активной подпиской:_ `{usersWithSubscription.Count()}`
                  """,
            replyMarkup: AdminKeyboard.Subscription,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);  
    }
}