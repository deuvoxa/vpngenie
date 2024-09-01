using System.Text.RegularExpressions;
using Telegram.Bot;
using vpngenie.API.TelegramBot.Handlers.User.Subscription;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;
using vpngenie.Domain.Entities;

namespace vpngenie.API.TelegramBot.States;

public static class ExpectingEmailState
{
    public static async Task Handle(
        IConfiguration configuration,
        ITelegramBotClient botClient, User user,
        long chatId, string messageText,
        ServerService serverService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(Service.ValidateEmail(messageText)))
        {
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text:
                "Пожалуйста, проверьте правильность введенного адреса электронной почты. Нам необходимо его корректное заполнение для отправки чека об оплате.",
                replyMarkup: new KeyboardBuilder().WithBackToSubscription().Build(),
                cancellationToken: cancellationToken);
        }
        else
        {
            Console.WriteLine($"получил мыло для оплаты: {Service.ValidateEmail(messageText)}");
            var yookassa = new YookassaClient(configuration);
            var paymentUrl = await yookassa.CreatePaymentAsync(100.0m, "Подписка на VPN Genie (30 дней)",
                "https://t.me/vpngenie_bot", chatId, Service.ValidateEmail(messageText));

            Console.WriteLine(user.MainMessageId);
            await botClient.EditMessageTextAsync(
                chatId: chatId,
                messageId: user.MainMessageId,
                text: "Ожидание оплаты...",
                replyMarkup: new KeyboardBuilder().WithUrlButton("Оплатить", paymentUrl).WithBackToSubscription()
                    .Build(),
                cancellationToken: cancellationToken);
        }
    }
}