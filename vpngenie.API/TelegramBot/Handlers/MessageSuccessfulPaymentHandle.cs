﻿using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.Handlers;

public static class SuccessfulPaymentHandle
{
    public static async Task Execute(ITelegramBotClient botClient, UserService userService,
        decimal amount, long userId, Guid paymentId)
    {
        var user = await userService.GetUserByTelegramIdAsync(userId);


        user.PaymentHistories.Add(new PaymentHistory
        {
            Amount = amount,
            PaymentDate = DateTime.UtcNow,
            PaymentId = paymentId
        });

        await userService.ExtendSubscriptionAsync(user, 31);
        var referrer = user.Referrer;
        if (referrer is not null)
            await userService.ExtendSubscriptionAsync(referrer, 6);

        await userService.UpdateUserAsync(user);

        var keyboard =new KeyboardBuilder()
            .WithButton("Получить конфиг","subscription-get-config")
            .WithBackToSubscription()
            .Build();
        
        await botClient.EditMessageTextAsync(
            chatId: userId,
            messageId: user.MainMessageId,
            text: $"""
                   *Платёж прошёл успешно!*

                   Ваша подписка активна до: {user.SubscriptionEndDate}
                   """,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown);
    }
}