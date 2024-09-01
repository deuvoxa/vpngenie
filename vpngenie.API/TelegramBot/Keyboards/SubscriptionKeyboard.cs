using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class SubscriptionKeyboard
{
    public static KeyboardBuilder WithBackToSubscription(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "subscription-menu");

    public static InlineKeyboardMarkup WithSubscription => new KeyboardBuilder()
        .WithButton("Получить конфиг", "subscription-get-config")
        .WithButton("История платежей", "subscription-payment-history")
        .WithButton("Продлить подписку", "subscription-activate")
        .WithBackToHome()
        .Build();

    public static InlineKeyboardMarkup WithoutSubscription => new KeyboardBuilder()
        .WithButton("Активировать подписку", "subscription-activate")
        .WithButton("История платежей", "subscription-payment-history")
        .WithBackToHome()
        .Build();

    public static InlineKeyboardMarkup BackConfig = new KeyboardBuilder()
        .WithButtons(new[]
        {
            ("Сменить регион", "subscription-choose-region"),
            ("Удалить конфиг", "subscription-remove-config")
        })
        .WithBackToSubscription()
        .Build();


    public static InlineKeyboardMarkup Home() => new KeyboardBuilder()
        .WithBackToSubscription()
        .Build();

    public static InlineKeyboardMarkup Home(int id) => new KeyboardBuilder()
        .WithButton("Вернуться назад", $"subscription-cancel-invoice-{id}")
        .Build();
}