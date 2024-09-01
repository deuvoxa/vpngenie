using Telegram.Bot.Types.ReplyMarkups;

namespace vpngenie.API.TelegramBot.Keyboards;

public static class PromotionKeyboard
{
    private static KeyboardBuilder WithBackToPromotions(this KeyboardBuilder builder)
        => builder.WithButton("Вернуться назад", "promotions-menu");

    public static InlineKeyboardMarkup Home => new KeyboardBuilder().WithBackToPromotions().Build();

    public static InlineKeyboardMarkup Referrals =>
        new KeyboardBuilder()
            .WithButton("Мои рефералы", "view-referrals")
            .WithBackToPromotions()
            .Build();

    public static InlineKeyboardMarkup WithDiscounts => new KeyboardBuilder()
        .WithButton("Реферальная программа", "referral_program")
        .WithButton("Текущие скидки и акции", "current_discounts")
        .WithButton("Промокоды", "promocodes")
        .WithBackToHome()
        .Build();

    public static InlineKeyboardMarkup WithoutDiscounts => new KeyboardBuilder()
        .WithButton("Реферальная программа", "referral_program")
        .WithButton("Промокоды", "promocodes")
        .WithBackToHome()
        .Build();
}