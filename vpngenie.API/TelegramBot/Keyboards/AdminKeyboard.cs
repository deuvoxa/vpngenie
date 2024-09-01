using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class AdminKeyboardExtensions
{
    public static KeyboardBuilder WithBackAdmin(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "admin-menu");
}

public static class AdminKeyboard
{
    public static InlineKeyboardMarkup Home => new KeyboardBuilder()
        .WithButtons(new[]
        {
            ("Подписки", "admin-subscription-menu"),
            ("Открытые тикеты", "admin-tickets-menu")
        })
        .WithButtons(new[]
        {
            ("Пользователи", "admin-users-menu"),
            ("Сервера", "admin-servers-menu")
        })
        .WithButton("Промокоды", "admin-promocodes-menu")
        .WithButton("Вернуться назад", "main-menu").Build();


    public static InlineKeyboardMarkup Back => new KeyboardBuilder().WithBackAdmin().Build();

    public static InlineKeyboardMarkup AddDays => new KeyboardBuilder().WithButtons(new[]
    {
        ("да", "admin-subscription-add-days"),
        ("нет", "admin-menu")
    }).Build();

    public static InlineKeyboardMarkup Subscription => new KeyboardBuilder()
        .WithButton("Активировать подписку", "admin-subscription-get-id")
        .WithButton("Вернуться назад", "admin-menu").Build();
}