using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingTelegramIdState
{
    public static async Task Handle(
        ITelegramBotClient botClient, UserService userService, 
        Domain.Entities.User user, long chatId, string messageText, 
        CancellationToken cancellationToken)
    {
        var userForAddDays = await userService.GetUserByTelegramIdAsync(long.Parse(messageText));
        if (userForAddDays is null)
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text: "Пользователь не найден",
                replyMarkup: AdminKeyboard.Back,
                cancellationToken: cancellationToken);
            return;
        }

        UserStates.User[chatId] = userForAddDays.TelegramId;
        var subscription = userForAddDays.SubscriptionIsActive
            ? $"Активна до {userForAddDays.SubscriptionEndDate}"
            : "Не активна";
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: $"""
                   Пользователь: `{userForAddDays.Username}`

                   Подписка: `{subscription}`

                   *Введите количество дней для активации подписки:*
                   """,
            replyMarkup: AdminKeyboard.Back,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        UserStates.State[chatId] = "ExpectingCountDays";
    }
}