using Humanizer;
using Humanizer.Localisation;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingCountDaysState
{
    public static async Task Handle(
        ITelegramBotClient botClient, Domain.Entities.User user, 
        UserService userService,
        long chatId, string messageText, 
        CancellationToken cancellationToken)
    {
        if (!int.TryParse(messageText, out var countDays))
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text: "Цифры пиши...",
                replyMarkup: AdminKeyboard.Back,
                cancellationToken: cancellationToken);
            return;
        }

        var userForSubscription = await userService.GetUserByTelegramIdAsync(UserStates.User[chatId]);
        
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: user.MainMessageId,
            text: $"Добавляю пользователю: `{userForSubscription.Username}`, {TimeSpan.FromDays(countDays).Humanize(4, maxUnit: TimeUnit.Year)} подписки?",
            replyMarkup: AdminKeyboard.AddDays,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
        UserStates.State[chatId] = string.Empty;
        UserStates.Days[chatId] = countDays;
    }
}