using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class MainKeyboard
{
    public static KeyboardBuilder WithBackToHome(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "main-menu");
    
    private static KeyboardBuilder WithHomeSettings(this KeyboardBuilder builder)
        => builder.WithButtons(new[]
            {
                ("Мои подписки", "subscription-menu"),
                ("Поддержка", "support-menu")
            })
            .WithButtons(new[]
            {
                ("Акции и бонусы", "promotions-menu"),
                ("Настройки", "settings-menu")
            })
            .WithButton("Информация", "information-menu");

    private static KeyboardBuilder WithHome(this KeyboardBuilder builder)
        => builder.WithButtons(new[]
            {
                ("Мои подписки", "subscription-menu"),
                ("Поддержка", "support-menu")
            })
            .WithButtons(new[]
            {
                ("Акции и бонусы", "promotions-menu"),
                ("Информация", "information-menu")
            });

    public static InlineKeyboardMarkup Back => new KeyboardBuilder().WithBackToHome().Build();

    public static InlineKeyboardMarkup Home => new KeyboardBuilder().WithHome().Build();
    public static InlineKeyboardMarkup HomeWithSettings => new KeyboardBuilder().WithHomeSettings().Build();

    public static InlineKeyboardMarkup GetAdminInlineKeyboard =>
        new KeyboardBuilder().WithHome().WithButton("Админка", "admin-menu").Build();
}