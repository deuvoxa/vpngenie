using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class MainKeyboard
{
    public static KeyboardBuilder WithBackToHome(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "main-menu");
    
    private static KeyboardBuilder WithSubscription(this KeyboardBuilder builder)
        => builder
            .WithButton("Моя подписка", "subscription-menu")
            .WithButtons([
                ("Акции и бонусы", "promotions-menu"),
                ("Информация", "information-menu")
            ])
            .WithButtons([
                ("Поддержка", "support-menu")
            ]);

    private static KeyboardBuilder WithHome(this KeyboardBuilder builder)
        => builder
            .WithButton("Активировать подписку", "subscription-activate")
            .WithButtons([
                ("Акции и бонусы", "promotions-menu"),
                ("Информация", "information-menu")
            ])
            .WithButtons([
                ("Поддержка", "support-menu")
            ]);

    public static InlineKeyboardMarkup Back => new KeyboardBuilder().WithBackToHome().Build();

    public static InlineKeyboardMarkup Home => new KeyboardBuilder().WithHome().Build();
    public static InlineKeyboardMarkup HomeWithSubscription => new KeyboardBuilder().WithSubscription().Build();

    public static InlineKeyboardMarkup GetAdminInlineKeyboard =>
        new KeyboardBuilder().WithSubscription().WithButton("Админка", "admin-menu").Build();
}