using Humanizer;
using Humanizer.Localisation;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using vpngenie.API.TelegramBot.Keyboards;
using vpngenie.Application.Services;

namespace vpngenie.API.TelegramBot.Handlers.User.Promotions;

public class HandlePromotions(
    ITelegramBotClient botClient,
    CallbackQuery callbackQuery,
    UserService userService,
    PromotionService promotionService,
    CancellationToken cancellationToken)
{
    public async Task ViewReferrals()
    {
        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);
        var referralsText = user.Referrals.OrderByDescending(r => r.PaymentHistories.Count).Aggregate("*Твои рефералы:*\n\n",
            (current, referral) =>
                current + $"От `{referral.Username}` получено {TimeSpan.FromDays(referral.PaymentHistories.Count * 6).Humanize(maxUnit: TimeUnit.Day, minUnit: TimeUnit.Day)}.\n\n");

        await EditMessage(referralsText,
            new KeyboardBuilder().WithButton("Вернуться назад", "referral_program").Build());
    }

    public async Task ReferralProgram()
    {
        var user = await userService.GetUserByTelegramIdAsync(callbackQuery.From.Id);

        var text =
            $"Каждый друг, которого вы пригласите в систему, останется с вами навсегда!\n\n" +
            $"Когда они приобретают подписку, вы получите 20% времени их подписки в качестве бонуса! 💼" +
            $"\n\nТвоя реферальная ссылка:\n" +
            $"`t.me/vpngenie_bot?start={user.TelegramId}`";

        await EditMessage(text, PromotionKeyboard.Referrals);
    }

    public async Task CurrentDiscounts()
    {
        var promotions = await promotionService.GetActivePromotionsAsync();

        var text = promotions.Count == 0
            ? "На данный момент акций и скидок нет."
            : "*Подробная информация о текущих акциях и скидках:*\n\n" + promotions.Aggregate("", (current, promo) =>
                current +
                $"*{promo.Title}*\n\n_{promo.Description}_\n\n*Даты проведения:*\n`{promo.StartDate:dd.MM.yyyy}` — `{promo.EndDate:dd.MM.yyyy}`\n\n");


        await EditMessage(text, PromotionKeyboard.Home);
    }

    private async Task EditMessage(string text, InlineKeyboardMarkup keyboard)
    {
        await botClient.EditMessageTextAsync(
            callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            text,
            replyMarkup: keyboard,
            parseMode: ParseMode.Markdown,
            cancellationToken: cancellationToken);
    }
}