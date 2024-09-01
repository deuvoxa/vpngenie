using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public static class AdminMenu
{
    public static async Task Main(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserService userService,
        CancellationToken cancellationToken)
    {
        try
        {
            var message = callbackQuery.Message!;
            var chatId = message.Chat.Id;
            UserStates.State[chatId] = string.Empty;
            await botClient.EditMessageTextAsync(
                chatId:chatId,
                messageId: message.MessageId,
                text: """
                      ***Админ меню***

                      Какую нибудь стату сюда выведу.
                      """,
                replyMarkup: AdminKeyboard.Home,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}