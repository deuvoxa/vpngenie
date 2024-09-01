using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class SupportKeyboard
{
    private static KeyboardBuilder WithBackToSupport(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "support-menu");

    public static KeyboardBuilder WithBackToFaq(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "faq");

    public static InlineKeyboardMarkup Home => new KeyboardBuilder().WithBackToSupport().Build();

    public static InlineKeyboardMarkup Main => new KeyboardBuilder()
        .WithButton("Часто задаваемые вопросы", "faq")
        .WithButton("Задать вопрос", "support-menu-questions")
        .WithBackToHome()
        .Build();
}