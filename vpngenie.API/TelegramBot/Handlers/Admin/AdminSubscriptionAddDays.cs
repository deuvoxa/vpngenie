using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.API.TelegramBot.States;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.Admin;

public static class AdminSubscriptionAddDays
{
    public static async Task Main(ITelegramBotClient botClient, CallbackQuery callbackQuery, UserService userService,
        CancellationToken cancellationToken)
    {
        var message = callbackQuery.Message!;
        var chatId = message.Chat.Id;
        // var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
        UserStates.State[chatId] = "ExpectingTelegramId";
        await botClient.EditMessageTextAsync(
            chatId: chatId,
            messageId: message.MessageId,
            text: "Введите ID пользователя, которому необходимо активировать подписку",
            replyMarkup: AdminKeyboard.Back,
            cancellationToken: cancellationToken);
    }

    public static async Task AddDays(ITelegramBotClient botClient, CallbackQuery callbackQuery,
        UserService userService, CancellationToken cancellationToken)
    {
        try
        {
            var message = callbackQuery.Message!;
            var chatId = message.Chat.Id;

            var userForSubscription = await userService.GetUserByTelegramIdAsync(UserStates.User[chatId]);

            Domain.Entities.User? user;

            if (userForSubscription.TelegramId == callbackQuery.From.Id)
                user = userForSubscription;
            else
                user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);


            await userService.ExtendSubscriptionAsync(userForSubscription, UserStates.Days[chatId]);
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text: $"Подписка для пользователся `{userForSubscription.Username}` успешно продлена!\n" +
                      $"Новая дата окончания подписки — `{userForSubscription.SubscriptionEndDate}`",
                replyMarkup: AdminKeyboard.Back,
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}